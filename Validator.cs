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
		public class ValidationError {
			public readonly Component component;
			public readonly Type componentType;
			public readonly FieldInfo fieldInfo;

			public ValidationError(Component component, Type componentType, FieldInfo fieldInfo) {
				this.component = component;
				this.componentType = componentType;
				this.fieldInfo = fieldInfo;
			}
		}

		public static HashSet<Assembly> kUnityAssemblies = new HashSet<Assembly>() {
			Assembly.GetAssembly(typeof(UnityEngine.MonoBehaviour)),
			Assembly.GetAssembly(typeof(UnityEngine.UI.Text)),
			Assembly.GetAssembly(typeof(UnityEditor.Editor))
		};

		public static IList<ValidationError> Validate(GameObject gameObject) {
			if (gameObject == null) {
				return null;
			}

			List<ValidationError> validationErrors = null;

			Queue<GameObject> queue = new Queue<GameObject>();
			queue.Enqueue(gameObject);

			while (queue.Count > 0) {
				GameObject current = queue.Dequeue();

				Component[] components = current.GetComponents<Component>();
				if (components == null) {
					continue;
				}

				foreach (Component c in components) {
					if (c == null) {
						continue;
					}

					Type componentType = c.GetType();

					// allow user defined ignores for namespaces
					bool inIgnoredNamespace = false;
					foreach (var validatorIgnoredNamespace in AssetDatabaseUtil.AllAssetsOfType<ValidatorIgnoredNamespace>()) {
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
						continue;
					}

					foreach (FieldInfo fieldInfo in TypeUtil.GetInspectorFields(componentType)
					.Where(f => typeof(UnityEventBase).IsAssignableFrom(f.FieldType))
					.Where(f => !Attribute.IsDefined(f, typeof(OptionalAttribute)) && !Attribute.IsDefined(f, typeof(HideInInspector)))) {
						// NOTE (darren): check UnityEvents for all classes
						UnityEventBase unityEvent = (UnityEventBase)fieldInfo.GetValue(c);
						if (unityEvent == null) {
							Debug.LogError("Unexpected null UnityEvent in GameObjectValidator!");
							continue;
						}

						for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++) {
							UnityEngine.Object target = unityEvent.GetPersistentTarget(i);
							string targetMethod = unityEvent.GetPersistentMethodName(i);

							if (target == null || string.IsNullOrEmpty(targetMethod) || target.GetType().GetMethod(targetMethod) == null) {
								validationErrors = validationErrors ?? new List<ValidationError>();
								validationErrors.Add(new ValidationError(c, componentType, fieldInfo));
								break;
							}
						}
					}

					if (kUnityAssemblies.Contains(componentType.Assembly)) {
						continue;
					}

					foreach (FieldInfo fieldInfo in TypeUtil.GetInspectorFields(componentType)
					.Where(f => !Attribute.IsDefined(f, typeof(OptionalAttribute)) && !Attribute.IsDefined(f, typeof(HideInInspector)))) {
						// NOTE (darren): this is to ignore fields that declared in super-classes out of our control (Unity)
						if (kUnityAssemblies.Contains(fieldInfo.DeclaringType.Assembly)) {
							continue;
						}

						bool isInvalid = fieldInfo.GetUnityEngineObjects(c).Any(o => o == null);
						if (isInvalid) {
							validationErrors = validationErrors ?? new List<ValidationError>();
							validationErrors.Add(new ValidationError(c, componentType, fieldInfo));
						}
					}
				}

				foreach (GameObject child in current.GetChildren()) {
					queue.Enqueue(child);
				}
			}

			return validationErrors;
		}
	}
}
