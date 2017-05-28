using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace DTValidator.Internal {
	public static class ValidationUtilTests {
		[Test]
		public static void ValidateAllGameObjectsInScenes_WorksAsExpected() {
			Scene scene = EditorSceneManager.GetSceneByName("TestMissingOutletsScene");
			IList<IValidationError> errors = ValidationUtil.ValidateAllGameObjectsInScenes(new Scene[] { scene });
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.GreaterThan(1));
		}

		[Test]
		public static void ValidateAllGameObjectsInScenes_EarlyExit_WorksAsExpected() {
			Scene scene = EditorSceneManager.GetSceneByName("TestMissingOutletsScene");
			IList<IValidationError> errors = ValidationUtil.ValidateAllGameObjectsInScenes(new Scene[] { scene }, earlyExitOnError: true);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
