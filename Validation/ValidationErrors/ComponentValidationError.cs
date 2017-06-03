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
		public readonly int ComponentLocalId;
		public readonly Type ComponentType;
		public readonly FieldInfo FieldInfo;
		public readonly object ContextObject;

		public ComponentValidationError(Component component, Type componentType, FieldInfo fieldInfo, object contextObject) {
			ComponentLocalId = component.GetLocalId();
			ComponentType = componentType;
			FieldInfo = fieldInfo;
			ContextObject = contextObject;
		}

		public override string ToString() {
			return string.Format("CVE ({0}=>{1}) context: {2}", FieldInfo.DeclaringType.Name, FieldInfo.Name, ContextObject);
		}


		// PRAGMA MARK - IValidationError Implementation
		int IValidationError.ObjectLocalId {
			get { return ComponentLocalId; }
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
