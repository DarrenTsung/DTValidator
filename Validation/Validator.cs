using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

using DTValidator.Internal;

namespace DTValidator {
	public static class Validator {
		// PRAGMA MARK - Static Public Interface
		public static IList<IValidationError> Validate(GameObject gameObject, bool recursive = false) {
			if (gameObject == null) {
				return null;
			}

			List<IValidationError> validationErrors = null;
			ValidateGameObjectInternal(gameObject, recursive, ref validationErrors);
			return validationErrors;
		}

		public static IList<IValidationError> Validate(ScriptableObject scriptableObject, bool recursive = false) {
			if (scriptableObject == null) {
				return null;
			}

			List<IValidationError> validationErrors = null;
			ValidateInternal(scriptableObject, recursive, ref validationErrors);
			return validationErrors;
		}


		// PRAGMA MARK - Static Internal
		private static HashSet<Assembly> kUnityAssemblies = new HashSet<Assembly>() {
			Assembly.GetAssembly(typeof(UnityEngine.MonoBehaviour)),
			Assembly.GetAssembly(typeof(UnityEngine.UI.Text)),
			Assembly.GetAssembly(typeof(UnityEditor.Editor))
		};

		private static IValidationError ComponentValidationErrorFactory(object obj, Type type, FieldInfo fieldInfo) {
			return new ComponentValidationError(obj as Component, type, fieldInfo);
		}

		private static IValidationError ObjectValidationErrorFactory(object obj, Type type, FieldInfo fieldInfo) {
			return new ObjectValidationError(obj, type, fieldInfo);
		}

		private static void ValidateGameObjectInternal(GameObject gameObject, bool recursive, ref List<IValidationError> validationErrors, HashSet<object> validatedObjects = null) {
			Queue<GameObject> queue = new Queue<GameObject>();
			queue.Enqueue(gameObject);

			if (recursive) {
				validatedObjects = validatedObjects ?? new HashSet<object>();
				validatedObjects.Add(gameObject);
			}

			while (queue.Count > 0) {
				GameObject current = queue.Dequeue();

				Component[] components = current.GetComponents<Component>();
				if (components == null) {
					continue;
				}

				foreach (Component c in components) {
					ValidateInternal(c, recursive, ref validationErrors, validatedObjects);
				}

				foreach (GameObject child in current.GetChildren()) {
					queue.Enqueue(child);
				}
			}
		}

		private static void ValidateInternal(object obj, bool recursive, ref List<IValidationError> validationErrors, HashSet<object> validatedObjects = null) {
			if (obj == null) {
				return;
			}

			if (validatedObjects != null) {
				if (validatedObjects.Contains(obj)) {
					return;
				}

				validatedObjects.Add(obj);
			}

			// TODO (darren): rename to objectType
			Type componentType = obj.GetType();

			Func<object, Type, FieldInfo, IValidationError> validationErrorFactory = null;
			if (typeof(Component).IsAssignableFrom(componentType)) {
				validationErrorFactory = ComponentValidationErrorFactory;
			} else {
				validationErrorFactory = ObjectValidationErrorFactory;
			}

			// allow user defined ignores for namespaces
			bool inIgnoredNamespace = false;
			foreach (var validatorIgnoredNamespace in ValidatorIgnoredNamespaceProvider.GetIgnoredNamespaces()) {
				if (validatorIgnoredNamespace == null) {
					Debug.LogWarning("Bad state - validatorIgnoredNamespace is null!");
					continue;
				}

				if (componentType.Namespace == null) {
					continue;
				}

				if (componentType.Namespace.Contains(validatorIgnoredNamespace.Namespace)) {
					inIgnoredNamespace = true;
					break;
				}
			}

			if (inIgnoredNamespace) {
				return;
			}

			foreach (FieldInfo fieldInfo in TypeUtil.GetInspectorFields(componentType)
			.Where(f => typeof(UnityEventBase).IsAssignableFrom(f.FieldType))
			.Where(f => !Attribute.IsDefined(f, typeof(OptionalAttribute)) && !Attribute.IsDefined(f, typeof(HideInInspector)))) {
				// NOTE (darren): check UnityEvents for all classes
				UnityEventBase unityEvent = (UnityEventBase)fieldInfo.GetValue(obj);
				if (unityEvent == null) {
					Debug.LogError("Unexpected null UnityEvent in GameObjectValidator!");
					continue;
				}

				for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++) {
					UnityEngine.Object target = unityEvent.GetPersistentTarget(i);
					string targetMethod = unityEvent.GetPersistentMethodName(i);

					if (target == null || string.IsNullOrEmpty(targetMethod) || target.GetType().GetMethod(targetMethod) == null) {
						validationErrors = validationErrors ?? new List<IValidationError>();
						validationErrors.Add(validationErrorFactory.Invoke(obj, componentType, fieldInfo));
						break;
					}
				}
			}

			if (kUnityAssemblies.Contains(componentType.Assembly)) {
				return;
			}

			foreach (FieldInfo fieldInfo in TypeUtil.GetInspectorFields(componentType)
			.Where(f => !Attribute.IsDefined(f, typeof(OptionalAttribute)) && !Attribute.IsDefined(f, typeof(HideInInspector)))) {
				// NOTE (darren): this is to ignore fields that declared in super-classes out of our control (Unity)
				if (kUnityAssemblies.Contains(fieldInfo.DeclaringType.Assembly)) {
					continue;
				}

				bool isInvalid = false;
				foreach (UnityEngine.Object fieldObject in fieldInfo.GetUnityEngineObjects(obj)) {
					if (fieldObject == null) {
						isInvalid = true;
						continue;
					}

					if (recursive) {
						GameObject fieldObjectAsGameObject = fieldObject as GameObject;
						if (fieldObjectAsGameObject != null) {
							PrefabType prefabType = PrefabUtility.GetPrefabType(fieldObjectAsGameObject);
							if (prefabType == PrefabType.Prefab) {
								validatedObjects = validatedObjects ?? new HashSet<object>() { obj };
								ValidateGameObjectInternal(fieldObjectAsGameObject, recursive, ref validationErrors, validatedObjects);
							}
						}
					}
				}
				if (isInvalid) {
					validationErrors = validationErrors ?? new List<IValidationError>();
					validationErrors.Add(validationErrorFactory.Invoke(obj, componentType, fieldInfo));
				}
			}
		}
	}
}
