#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
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
		public static IList<IValidationError> ValidateAllGameObjectsInBuildScenes(bool earlyExitOnError = false) {
			return ValidateAllGameObjectsInScenes(GetBuildScenes(), earlyExitOnError);
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


		// PRAGMA MARK - Internal
		private static IEnumerable<Scene> GetBuildScenes() {
			int buildSceneCount = EditorSceneManager.sceneCountInBuildSettings;
			for (int i = 0; i < buildSceneCount; i++) {
				yield return EditorSceneManager.GetSceneByBuildIndex(i);
			}
		}
	}
}
#endif
