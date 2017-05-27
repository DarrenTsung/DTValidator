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
	public static class NestedGameObjectValidationTests {
		private class OutletComponent : MonoBehaviour {
			public GameObject Outlet;
		}

		[Test]
		public static void MissingNestedOutlet_ReturnsErrors() {
			GameObject gameObjectA = new GameObject("A");
			GameObject gameObjectB = new GameObject("B");

			var outletComponent = gameObjectB.AddComponent<OutletComponent>();
			outletComponent.Outlet = null;

			gameObjectB.transform.SetParent(gameObjectA.transform);

			IList<IValidationError> errors = Validator.Validate(gameObjectA);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
