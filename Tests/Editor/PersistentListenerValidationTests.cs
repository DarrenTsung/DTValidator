using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using NUnit.Framework;

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

		private class UnityEventOutletComponent : MonoBehaviour {
			public UnityEvent EventOutlet = new UnityEvent();
		}

		private class SimpleEventHandlerComponent : MonoBehaviour {
			public void ResolveEvent() {}
		}

		[Test]
		public static void ValidPersistentListener_ReturnsNoErrors() {
			GameObject go = new GameObject();
			var simpleEventHandlerComponent = go.AddComponent<SimpleEventHandlerComponent>();
			var unityEventOutletComponent = go.AddComponent<UnityEventOutletComponent>();
			AddPersistentListener(unityEventOutletComponent.EventOutlet, simpleEventHandlerComponent, "ResolveEvent");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void InvalidPersistentListener_ReturnsError() {
			GameObject go = new GameObject();
			var simpleEventHandlerComponent = go.AddComponent<SimpleEventHandlerComponent>();
			var unityEventOutletComponent = go.AddComponent<UnityEventOutletComponent>();
			AddPersistentListener(unityEventOutletComponent.EventOutlet, simpleEventHandlerComponent, "ResolveEvento");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		[Test]
		public static void InvalidUnityEnginePersistentListener_ReturnsError() {
			GameObject go = new GameObject();
			var button = go.AddComponent<UnityEngine.UI.Button>();
			AddPersistentListener(button.onClick, go as UnityEngine.Object, "Invalido");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}

		private class ComplexEventHandlerComponent : MonoBehaviour {
			public void ResolveEvent() {}
			public void ResolveEvent(int i) {}
		}

		[Test]
		public static void EventWithSameName_ResolvesCorrectly() {
			GameObject go = new GameObject();
			var complexEventHandlerComponent = go.AddComponent<ComplexEventHandlerComponent>();
			var unityEventOutletComponent = go.AddComponent<UnityEventOutletComponent>();
			AddPersistentListener(unityEventOutletComponent.EventOutlet, complexEventHandlerComponent, "ResolveEvent");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Null);
		}

		private class IntUnityEvent : UnityEvent<int> {
		}

		private class IntUnityEventOutletComponent : MonoBehaviour {
			public IntUnityEvent EventOutlet = new IntUnityEvent();
		}

		[Test]
		public static void IntEventWithSameName_ResolvesCorrectly() {
			GameObject go = new GameObject();
			var complexEventHandlerComponent = go.AddComponent<ComplexEventHandlerComponent>();
			var intUnityEventOutletComponent = go.AddComponent<IntUnityEventOutletComponent>();
			AddPersistentListener(intUnityEventOutletComponent.EventOutlet, complexEventHandlerComponent, "ResolveEvent");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public static void IntEventWithWrongParameters_ReturnsErrors() {
			GameObject go = new GameObject();
			var simpleEventHandlerComponent = go.AddComponent<SimpleEventHandlerComponent>();
			var intUnityEventOutletComponent = go.AddComponent<IntUnityEventOutletComponent>();
			AddPersistentListener(intUnityEventOutletComponent.EventOutlet, simpleEventHandlerComponent, "ResolveEvent");
			IList<IValidationError> errors = Validator.Validate(go);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
