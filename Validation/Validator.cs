#if UNITY_EDITOR
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
		public static IList<IValidationError> Validate(GameObject gameObject, object contextObject = null, bool recursive = false, List<IValidationError> validationErrors = null) {
			if (gameObject == null) {
				return null;
			}

			if (contextObject == null) {
				contextObject = gameObject;
			}

			ValidateGameObjectInternal(gameObject, contextObject, recursive, ref validationErrors);
			return validationErrors;
		}

		public static IList<IValidationError> Validate(ScriptableObject scriptableObject, bool recursive = false, List<IValidationError> validationErrors = null) {
			if (scriptableObject == null) {
				return null;
			}

			ValidateInternal(scriptableObject, scriptableObject, recursive, ref validationErrors);
			return validationErrors;
		}


		// PRAGMA MARK - Static Internal
		private static HashSet<Assembly> kUnityAssemblies = new HashSet<Assembly>() {
			Assembly.GetAssembly(typeof(UnityEngine.MonoBehaviour)),
			Assembly.GetAssembly(typeof(UnityEngine.UI.Text)),
			Assembly.GetAssembly(typeof(UnityEditor.Editor))
		};

		private static void ValidateGameObjectInternal(GameObject gameObject, object contextObject, bool recursive, ref List<IValidationError> validationErrors, HashSet<object> validatedObjects = null) {
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
					ValidateInternal(c, contextObject, recursive, ref validationErrors, validatedObjects);
				}

				foreach (GameObject child in current.GetChildren()) {
					queue.Enqueue(child);
				}
			}
		}

		private static void ValidateInternal(object obj, object contextObject, bool recursive, ref List<IValidationError> validationErrors, HashSet<object> validatedObjects = null) {
			if (obj == null) {
				return;
			}

			if (validatedObjects != null) {
				if (validatedObjects.Contains(obj)) {
					return;
				}

				validatedObjects.Add(obj);
			}

			Type objectType = obj.GetType();

			// allow user defined ignores for namespaces
			bool inIgnoredNamespace = false;
			foreach (var validatorIgnoredNamespace in ValidatorIgnoredNamespaceProvider.GetIgnoredNamespaces()) {
				if (validatorIgnoredNamespace == null) {
					Debug.LogWarning("Bad state - validatorIgnoredNamespace is null!");
					continue;
				}

				if (objectType.Namespace == null) {
					continue;
				}

				if (objectType.Namespace.Contains(validatorIgnoredNamespace.Namespace)) {
					inIgnoredNamespace = true;
					break;
				}
			}

			if (inIgnoredNamespace) {
				return;
			}

			foreach (FieldInfo fieldInfo in TypeUtil.GetInspectorFields(objectType)
			.Where(f => typeof(UnityEventBase).IsAssignableFrom(f.FieldType))
			.Where(f => !Attribute.IsDefined(f, typeof(OptionalAttribute)) && !Attribute.IsDefined(f, typeof(HideInInspector)))) {
				// NOTE (darren): check UnityEvents for all classes
				UnityEventBase unityEvent = (UnityEventBase)fieldInfo.GetValue(obj);
				if (unityEvent == null) {
					Debug.LogError("Unexpected null UnityEvent in GameObjectValidator!");
					continue;
				}

				// NOTE (darren): we grab the base type UnityEvent<T1>, UnityEvent<T1, T2> if possible...
				Type[] argumentTypes = null;
				Type fieldType = fieldInfo.FieldType;
				while (fieldType != null) {
					if (fieldType.IsGenericType) {
						Type fieldGenericType = fieldType.GetGenericTypeDefinition();
						if (fieldGenericType != typeof(UnityEvent<>) && fieldGenericType != typeof(UnityEvent<,>) && fieldGenericType != typeof(UnityEvent<,,>)) {
							// if not at UnityEvent<> generic class then keep going up type tree
							continue;
						}

						argumentTypes = fieldType.GetGenericArguments();
						break;
					}
					fieldType = fieldType.BaseType;
				}

				for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++) {
					UnityEngine.Object target = unityEvent.GetPersistentTarget(i);
					string targetMethod = unityEvent.GetPersistentMethodName(i);

					if (target == null || string.IsNullOrEmpty(targetMethod) || target.GetType().GetMethod(targetMethod, argumentTypes ?? new Type[0]) == null) {
						validationErrors = validationErrors ?? new List<IValidationError>();
						validationErrors.Add(ValidationErrorFactory.Create(obj, objectType, fieldInfo, contextObject));
						break;
					}
				}
			}

			if (kUnityAssemblies.Contains(objectType.Assembly)) {
				return;
			}

			foreach (FieldInfo fieldInfo in TypeUtil.GetInspectorFields(objectType)
			.Where(f => !Attribute.IsDefined(f, typeof(OptionalAttribute)) && !Attribute.IsDefined(f, typeof(HideInInspector)))) {
				// NOTE (darren): this is to ignore fields that declared in super-classes out of our control (Unity)
				if (kUnityAssemblies.Contains(fieldInfo.DeclaringType.Assembly)) {
					continue;
				}

				int index = 0;
				foreach (UnityEngine.Object fieldObject in fieldInfo.GetUnityEngineObjects(obj)) {
					if (fieldObject == null) {
						validationErrors = validationErrors ?? new List<IValidationError>();
						if (fieldInfo.FieldType.IsClass) {
							validationErrors.Add(ValidationErrorFactory.Create(obj, objectType, fieldInfo, contextObject));
						} else {
							validationErrors.Add(ValidationErrorFactory.Create(obj, objectType, fieldInfo, contextObject, index));
						}
						index++;
						continue;
					}

					if (recursive) {
						GameObject fieldObjectAsGameObject = fieldObject as GameObject;
						if (fieldObjectAsGameObject != null) {
							PrefabType prefabType = PrefabUtility.GetPrefabType(fieldObjectAsGameObject);
							if (prefabType == PrefabType.Prefab) {
								// switch context to the prefab we just recursed to
								object newContextObject = fieldObjectAsGameObject;

								validatedObjects = validatedObjects ?? new HashSet<object>() { obj };
								ValidateGameObjectInternal(fieldObjectAsGameObject, newContextObject, recursive, ref validationErrors, validatedObjects);
							}
						}

						ScriptableObject fieldObjectAsScriptableObject = fieldObject as ScriptableObject;
						if (fieldObjectAsScriptableObject != null) {
							// switch context to the scriptable object we just recursed to
							object newContextObject = fieldObjectAsScriptableObject;

							validatedObjects = validatedObjects ?? new HashSet<object>() { obj };
							ValidateInternal(fieldObjectAsScriptableObject, newContextObject, recursive, ref validationErrors, validatedObjects);
						}
					}
					index++;
				}
			}
		}
	}
}
#endif
