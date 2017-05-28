using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using UnityEngine.UI;

using NUnit.Framework;
using UnityEngine.TestTools;

namespace DTValidator.Internal {
	public static class RecursiveValidationTests {
		private class OutletComponent : MonoBehaviour {
			public GameObject Outlet;
		}

		[Test]
		public static void RecursivePrefabWithMissingOutlet_ReturnsErrors() {
			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<OutletComponent>();

			GameObject missingOutletPrefab = Resources.Load<GameObject>("DTValidatorTests/TestMissingOutletPrefab");
			outletComponent.Outlet = missingOutletPrefab;

			IList<IValidationError> errors = Validator.Validate(gameObject, recursive: true);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
