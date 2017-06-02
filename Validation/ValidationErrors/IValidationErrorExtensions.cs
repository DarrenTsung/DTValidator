#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

using UnityEditor;

namespace DTValidator.Internal {
	public static class IValidationErrorExtensions {
		public static string GetContextObjectName(this IValidationError validationError) {
			object context = validationError.ContextObject;
			if (context == null) {
				Debug.LogWarning("Cannot get name for null context! Error: " + validationError);
				return null;
			}

			try {
				Scene scene = (Scene)context;
				if (scene.IsValid()) {
					string sceneName = Path.GetFileName(scene.path);
					return string.Format("{0} ({1})", sceneName, (validationError.Object as UnityEngine.Object).name);
				}
			} catch {}

			UnityEngine.Object contextObject = context as UnityEngine.Object;
			if (contextObject == null) {
				Debug.LogWarning("Cannot get name of null UnityEngine.Object context: " + contextObject);
				return null;
			}

			return contextObject.name;
		}

		public static void SelectInEditor(this IValidationError validationError) {
			bool selected = SelectObjectInEditor(validationError);
			if (!selected) {
				SelectContextInEditor(validationError);
			}
		}

		private static bool SelectObjectInEditor(IValidationError validationError) {
			UnityEngine.Component objectAsComponent = validationError.Object as UnityEngine.Component;
			if (objectAsComponent == null) {
				return false;
			}

			Type componentType = objectAsComponent.GetType();
			UnityEngine.Object[] objects = UnityEngine.Object.FindObjectsOfType(componentType);
			bool foundInLoadedScenes = objects.Any(o => o == objectAsComponent);
			if (!foundInLoadedScenes) {
				return false;
			}

			Selection.activeTransform = objectAsComponent.transform;
			EditorGUIUtility.PingObject(objectAsComponent);
			return true;
		}

		private static void SelectContextInEditor(IValidationError validationError) {
			object context = validationError.ContextObject;
			if (context == null) {
				Debug.LogWarning("Cannot select context for null context! Error: " + validationError);
				return;
			}

			// NOTE (darren): need to get the SceneAsset from the Scene
			try {
				Scene scene = (Scene)context;
				if (scene.IsValid()) {
					context = AssetDatabase.LoadMainAssetAtPath(scene.path);
				}
			} catch {}

			UnityEngine.Object contextObject = context as UnityEngine.Object;
			if (contextObject == null) {
				Debug.LogWarning("Cannot select null UnityEngine.Object context: " + contextObject);
				return;
			}

			Selection.activeObject = contextObject;
			EditorGUIUtility.PingObject(contextObject);
		}
	}
}
#endif
