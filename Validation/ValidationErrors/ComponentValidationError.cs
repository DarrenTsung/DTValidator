using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace DTValidator.Internal {
	public class ComponentValidationError : IValidationError {
		// PRAGMA MARK - Public Interface
		public readonly Component Component;
		public readonly Type ComponentType;
		public readonly FieldInfo FieldInfo;
		public readonly object ContextObject;

		public ComponentValidationError(Component component, Type componentType, FieldInfo fieldInfo, object contextObject) {
			Component = component;
			ComponentType = componentType;
			FieldInfo = fieldInfo;
			ContextObject = contextObject;
		}

		public override string ToString() {
			return string.Format("CVE (Component: {0}=>{2} ({1})) context: {3}", Component.gameObject.FullName(), FieldInfo.DeclaringType.Name, FieldInfo.Name, ContextObject);
		}


		// PRAGMA MARK - IValidationError Implementation
		object IValidationError.Object {
			get { return Component; }
		}

		Type IValidationError.ObjectType {
			get { return ComponentType; }
		}

		FieldInfo IValidationError.FieldInfo {
			get { return FieldInfo; }
		}

		object IValidationError.ContextObject {
			get { return ContextObject; }
		}
	}
}
