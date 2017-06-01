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
	public class ValidatorWindow : EditorWindow {
		// PRAGMA MARK - Static Public Interface
		[MenuItem("Window/DTValidator Window", priority=100)]
		public static void ShowWindow() {
			var window = EditorWindow.GetWindow<ValidatorWindow>(utility: false, title: "Validator Window");
			window.minSize = new Vector2(kMinWindowWidth, kMinWindowHeight);
		}


		// PRAGMA MARK - Internal
		private const float kMinWindowWidth = 410.0f;
		private const float kMinWindowHeight = 120.0f;

		private const float kTopBarSize = 20.0f;

		private readonly List<IValidationError> validationErrors_ = new List<IValidationError>();
		private float? validationTimeInMS_ = null;

        private static Vector2 ScrollPosition_ {
            get {
                if (!EditorPrefs.HasKey("ValidatorWindow::scrollPosition_")) {
                    return Vector2.zero;
                }

                return JsonUtility.FromJson<Vector2>(EditorPrefs.GetString("ValidatorWindow::scrollPosition_", ""));
            }
            set { EditorPrefs.SetString("ValidatorWindow::scrollPosition_", JsonUtility.ToJson(value)); }
        }

		private static bool ValidateScriptableObjects_ {
			get { return EditorPrefs.GetBool("ValidatorWindow::ValidateScriptableObjects_", defaultValue: true); }
			set { EditorPrefs.SetBool("ValidatorWindow::ValidateScriptableObjects_", value); }
		}

		private static bool ValidateSavedScenes_ {
			get { return EditorPrefs.GetBool("ValidatorWindow::ValidateSavedScenes_", defaultValue: true); }
			set { EditorPrefs.SetBool("ValidatorWindow::ValidateSavedScenes_", value); }
		}

		private static bool ValidatePrefabsInResources_ {
			get { return EditorPrefs.GetBool("ValidatorWindow::ValidatePrefabsInResources_", defaultValue: true); }
			set { EditorPrefs.SetBool("ValidatorWindow::ValidatePrefabsInResources_", value); }
		}

        private void OnGUI() {
            if (Event.current.type == EventType.Layout) {
                // only modify s_Results on Layout event
            }

            EditorGUILayout.BeginVertical(GUILayout.Height(kTopBarSize));
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Validate:", GUILayout.Width(50.0f));
                    ValidateScriptableObjects_ = GUILayout.Toggle(ValidateScriptableObjects_, "Scriptable Objects");
                    ValidateSavedScenes_ = GUILayout.Toggle(ValidateSavedScenes_, "Saved Scenes");
                    ValidatePrefabsInResources_ = GUILayout.Toggle(ValidatePrefabsInResources_, "Prefabs in Resources");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
					string buttonTitle = "Validate!";
					if (validationTimeInMS_ != null) {
						buttonTitle += string.Format(" ({0:0.00}s)", validationTimeInMS_.Value / 100.0f);
					}
					if (GUILayout.Button(buttonTitle)) {
						Validate();
					}
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            ScrollPosition_ = EditorGUILayout.BeginScrollView(ScrollPosition_, GUILayout.Height(position.height - kTopBarSize));
				int index = 0;
				foreach (IValidationError error in validationErrors_) {
					Color color = index % 2 == 0 ? ColorUtil.HexStringToColor("#ff6969") : ColorUtil.HexStringToColor("#f05555");

					EditorGUILayout.BeginVertical(EditorGUIStyleUtil.StyleWithBackgroundColor(color));
						EditorGUILayout.BeginVertical();
							EditorGUILayout.LabelField("Title", EditorGUIStyleUtil.CachedLabelTitleStyle(), EditorGUIStyleUtil.TitleHeight);

							var horizontalStyle = EditorGUIStyleUtil.CachedStyle("DTValidatorWindow::HorizontalMarginStyle", GUIStyle.none, (style) => {
								style.padding.bottom = 2;
								style.margin.bottom = 3;
							});
							EditorGUILayout.BeginHorizontal(horizontalStyle);
								EditorGUILayout.LabelField(error.ToString());
								if (GUILayout.Button("Jump To", EditorGUIStyleUtil.CachedAlignedButtonStyle(), GUILayout.ExpandWidth(false))) {
									// TODO (darren): implement this!
									Debug.Log("Jump to error!");
								}
							EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();

					index++;
				}
            EditorGUILayout.EndScrollView();
        }

		private void Validate() {
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			validationErrors_.Clear();

			if (ValidateScriptableObjects_) {
				IList<IValidationError> errors = ValidationUtil.ValidateAllSavedScriptableObjects(earlyExitOnError: false);
				if (errors != null) {
					validationErrors_.AddRange(errors);
				}
			}

			if (ValidateSavedScenes_) {
				IList<IValidationError> errors = ValidationUtil.ValidateAllGameObjectsInSavedScenes(earlyExitOnError: false);
				if (errors != null) {
					validationErrors_.AddRange(errors);
				}
			}

			if (ValidatePrefabsInResources_) {
				// TODO (darren): do this
				// IList<IValidationError> errors = ValidationUtil.ValidateAllSavedScriptableObjects(earlyExitOnError: false);
				// if (errors != null) {
				// 	validationErrors_.AddRange(errors);
				// }
			}

			stopwatch.Stop();
			validationTimeInMS_ = stopwatch.ElapsedMilliseconds;
		}
	}
}
#endif
