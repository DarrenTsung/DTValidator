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

namespace DTValidator.TestIgnore {
	public class IgnoredOutletComponent : MonoBehaviour {
		public GameObject Outlet;
	}
}

namespace DTValidator.Internal {
	using DTValidator.TestIgnore;

	public static class ValidatorIgnoredNamespaceTests {
		private static IEnumerable<ValidatorIgnoredNamespace> IgnoredOutletComponentNamespaceProvider() {
			var ignoredNamespace = ScriptableObject.CreateInstance<ValidatorIgnoredNamespace>();
			ignoredNamespace.Namespace = "DTValidator.TestIgnore";

			return new ValidatorIgnoredNamespace[] { ignoredNamespace };
		}

		[Test]
		public static void IgnoredMissingOutlet_ReturnsNoErrors() {
			ValidatorIgnoredNamespaceProvider.SetCurrentProvider(IgnoredOutletComponentNamespaceProvider);

			GameObject gameObject = new GameObject();

			var outletComponent = gameObject.AddComponent<IgnoredOutletComponent>();
			outletComponent.Outlet = null;

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);

			ValidatorIgnoredNamespaceProvider.ClearCurrentProvider();
		}
	}
}
