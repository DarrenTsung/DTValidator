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
		// PRAGMA MARK - Public Interface
		public static Texture2D GetContextIcon(this IValidationError validationError) {
			if (validationError.ContextObject is GameObject) {
				return DTValidatorIcons.PrefabIcon;
			} else if (validationError.ContextObject is SceneAsset) {
				return DTValidatorIcons.SceneIcon;
			} else if (validationError.ContextObject is ScriptableObject) {
				return DTValidatorIcons.ScriptableObjectIcon;
			} else {
				Debug.LogWarning("Failed to get image because context object is not recognized type: " + validationError.ContextObject + "!");
				return Texture2DUtil.ClearTexture;
			}
		}

		public static string GetContextObjectName(this IValidationError validationError) {
			object context = validationError.ContextObject;
			if (context == null) {
				Debug.LogWarning("Cannot get name for null context! Error: " + validationError);
				return null;
			}

			UnityEngine.Object contextObject = context as UnityEngine.Object;
			if (contextObject == null) {
				Debug.LogWarning("Cannot get name of null UnityEngine.Object context: " + contextObject);
				return null;
			}

			string path = AssetDatabase.GetAssetPath(contextObject);
			return Path.GetFileName(path);
		}

		public static void SelectInEditor(this IValidationError validationError) {
			bool selected = SelectObjectInEditor(validationError);
			if (!selected) {
				SelectContextInEditor(validationError);
			}
		}


		// PRAGMA MARK - Internal
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
