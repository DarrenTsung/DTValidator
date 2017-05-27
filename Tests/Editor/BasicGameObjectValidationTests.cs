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
			IList<Validator.ValidationError> errors = Validator.Validate(emptyGameObject);
			Assert.That(errors, Is.Null);
		}
	}
}
