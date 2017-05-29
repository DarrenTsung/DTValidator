#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace DTValidator {
	public static class ValidationUtil {
		// PRAGMA MARK - Static Public Interface
		public static IList<IValidationError> ValidateAllSavedScriptableObjects(bool earlyExitOnError = false) {
			return ValidateAllScriptableObjects(GetSavedScriptableObjects(), earlyExitOnError);
		}

		public static IList<IValidationError> ValidateAllScriptableObjects(IEnumerable<ScriptableObject> scriptableObjects, bool earlyExitOnError = false) {
			List<IValidationError> validationErrors = new List<IValidationError>();

			foreach (ScriptableObject scriptableObject in scriptableObjects) {
				Validator.Validate(scriptableObject, recursive: true, validationErrors: validationErrors);
				if (earlyExitOnError && validationErrors.Count > 0) {
					return validationErrors;
				}
			}

			return validationErrors;
		}

		public static IList<IValidationError> ValidateAllGameObjectsInSavedScenes(bool earlyExitOnError = false) {
			return ValidateAllGameObjectsInScenes(GetSavedScenes(), earlyExitOnError);
		}

		public static IList<IValidationError> ValidateAllGameObjectsInScenes(IEnumerable<Scene> scenes, bool earlyExitOnError = false) {
			List<IValidationError> validationErrors = new List<IValidationError>();

			foreach (Scene scene in scenes) {
				GameObject[] rootObjects = scene.GetRootGameObjects();
				foreach (GameObject rootObject in rootObjects) {
					Validator.Validate(rootObject, recursive: true, validationErrors: validationErrors);
					if (earlyExitOnError && validationErrors.Count > 0) {
						return validationErrors;
					}
				}
			}

			return validationErrors;
		}

		// TODO (darren): add utility function for validating all prefabs in Resources

		// PRAGMA MARK - Internal
		private static IEnumerable<Scene> GetSavedScenes() {
			string[] guids = AssetDatabase.FindAssets("t:Scene");
			foreach (string guid in guids) {
				string path = AssetDatabase.GUIDToAssetPath(guid);

				// NOTE (darren): we want to ignore all the test scenes in
				// DTValidator folder.. can we do this a better way?
				if (path.Contains("DTValidator")) {
					continue;
				}

				yield return EditorSceneManager.OpenScene(path);
			}
		}

		private static IEnumerable<ScriptableObject> GetSavedScriptableObjects() {
			string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
			foreach (string guid in guids) {
				string path = AssetDatabase.GUIDToAssetPath(guid);
				yield return AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject)) as ScriptableObject;
			}
		}
	}
}
#endif
