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
	public static class PersistentListenerValidationTests {
		private static readonly FieldInfo kCallsField = typeof(UnityEventBase).GetField("m_PersistentCalls", BindingFlags.NonPublic | BindingFlags.Instance);
		private static readonly MethodInfo kDirtyPersistentCallsMethod = typeof(UnityEventBase).GetMethod("DirtyPersistentCalls", BindingFlags.NonPublic | BindingFlags.Instance);
		private static readonly Type kPersistentCallGroupType = Assembly.GetAssembly(typeof(UnityEventBase)).GetTypes().First(t => t.Name == "PersistentCallGroup");
		private static readonly MethodInfo kRegisterListenerMethod = kPersistentCallGroupType.GetMethod("RegisterEventPersistentListener");
		private static readonly MethodInfo kAddListenerMethod = kPersistentCallGroupType.GetMethod("AddListener", types: new Type[0]);

		public static void AddPersistentListener(UnityEventBase unityEvent, UnityEngine.Object targetObj, string methodName) {
			var persistentCalls = kCallsField.GetValue(unityEvent);
			kAddListenerMethod.Invoke(persistentCalls, null);
			kRegisterListenerMethod.Invoke(persistentCalls, new object[] { 0, targetObj, methodName });
			kDirtyPersistentCallsMethod.Invoke(unityEvent, null);
		}

		[Test]
		public static void ValidPersistentListener_ReturnsNoErrors() {
			GameObject go = new GameObject();
			var button = go.AddComponent<Button>();
			AddPersistentListener(button.onClick, go as UnityEngine.Object, "SetActive");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void InvalidPersistentListener_ReturnsError() {
			GameObject go = new GameObject();
			var button = go.AddComponent<Button>();
			AddPersistentListener(button.onClick, go as UnityEngine.Object, "Invalido");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
