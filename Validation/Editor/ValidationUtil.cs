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
			string oldActiveScenePath = EditorSceneManager.GetActiveScene().path;
			string[] oldScenePaths = new string[EditorSceneManager.sceneCount];
			for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
				oldScenePaths[i] = EditorSceneManager.GetSceneAt(i).path;
			}

			IList<IValidationError> validationErrors = ValidateAllGameObjectsInScenes(GetSavedScenes(), earlyExitOnError);

			bool first = true;
			foreach (string scenePath in oldScenePaths) {
				Scene scene = EditorSceneManager.OpenScene(scenePath, first ? OpenSceneMode.Single : OpenSceneMode.Additive);
				if (scenePath == oldActiveScenePath) {
					EditorSceneManager.SetActiveScene(scene);
				}

				first = false;
			}

			return validationErrors;
		}

		public static IList<IValidationError> ValidateAllGameObjectsInScenes(IEnumerable<Scene> scenes, bool earlyExitOnError = false) {
			List<IValidationError> validationErrors = new List<IValidationError>();

			foreach (Scene scene in scenes) {
				// NOTE (darren): use SceneAsset instead of Scene as the context object
				// because scene is a struct and was being lost when returning out-of-scope as
				// part of IValidationError
				SceneAsset sceneAsset = AssetDatabase.LoadMainAssetAtPath(scene.path) as SceneAsset;
				if (sceneAsset == null) {
					Debug.LogWarning("Cannot validate game objects with missing SceneAsset at path: " + scene.path);
					continue;
				}

				GameObject[] rootObjects = scene.GetRootGameObjects();
				foreach (GameObject rootObject in rootObjects) {
					Validator.Validate(rootObject, contextObject: sceneAsset, recursive: true, validationErrors: validationErrors);
					if (earlyExitOnError && validationErrors.Count > 0) {
						return validationErrors;
					}
				}
			}

			return validationErrors;
		}

		public static IList<IValidationError> ValidateAllGameObjectsInResources(bool earlyExitOnError = false) {
			List<IValidationError> validationErrors = new List<IValidationError>();

			foreach (GameObject prefab in Resources.LoadAll("", typeof(GameObject))) {
				Validator.Validate(prefab, recursive: true, validationErrors: validationErrors);
				if (earlyExitOnError && validationErrors.Count > 0) {
					return validationErrors;
				}
			}

			return validationErrors;
		}


		// PRAGMA MARK - Internal
		private static IEnumerable<Scene> GetSavedScenes() {
			string[] guids = AssetDatabase.FindAssets("t:Scene");
			foreach (string guid in guids) {
				yield return EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(guid));
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
