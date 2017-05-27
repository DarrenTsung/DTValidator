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
	public static class BasicGameObjectValidationTests {
		[Test]
		public static void EmptyGameObject_ReturnsNoErrors() {
			GameObject emptyGameObject = new GameObject();
			IList<IValidationError> errors = Validator.Validate(emptyGameObject);
			Assert.That(errors, Is.Null);
		}

		private class OutletComponent : MonoBehaviour {
			public GameObject Outlet;
		}

		[Test]
		public static void FilledOutlet_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<OutletComponent>();
			outletComponent.Outlet = gameObject;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void MissingOutlet_ReturnsErrors() {
			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<OutletComponent>();
			outletComponent.Outlet = null;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
