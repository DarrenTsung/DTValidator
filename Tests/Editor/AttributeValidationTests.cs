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
	public static class AttributeValidationTests {
		private class HiddenOutletComponent : MonoBehaviour {
			[HideInInspector]
			public GameObject DynamicSerializedOutlet;
		}

		[Test]
		public static void MissingHiddenOutlets_ReturnsNoErrors() {
			GameObject emptyGameObject = new GameObject();
			emptyGameObject.AddComponent<HiddenOutletComponent>();
			IList<IValidationError> errors = Validator.Validate(emptyGameObject);
			Assert.That(errors, Is.Null);
		}

		private class OptionalOutletComponent : MonoBehaviour {
			[Optional]
			public GameObject Outlet;
		}

		[Test]
		public static void OptionalOutlets_ReturnsNoErrors() {
			GameObject emptyGameObject = new GameObject();
			emptyGameObject.AddComponent<OptionalOutletComponent>();
			IList<IValidationError> errors = Validator.Validate(emptyGameObject);
			Assert.That(errors, Is.Null);
		}
	}
}
