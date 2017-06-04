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
		public readonly MemberInfo MemberInfo;
		public readonly object ContextObject;

		public readonly string ComponentPath;

		public ComponentValidationError(Component component, Type componentType, MemberInfo memberInfo, object contextObject) {
			ComponentLocalId = component.GetLocalId();
			ComponentPath = component.gameObject.FullName();
			ComponentType = componentType;
			MemberInfo = memberInfo;
			ContextObject = contextObject;
		}

		public override string ToString() {
			return string.Format("CVE ({0}=>{1}) context: {2}", MemberInfo.DeclaringType.Name, MemberInfo.Name, ContextObject);
		}


		// PRAGMA MARK - IValidationError Implementation
		int IValidationError.ObjectLocalId {
			get { return ComponentLocalId; }
		}

		Type IValidationError.ObjectType {
			get { return ComponentType; }
		}

		MemberInfo IValidationError.MemberInfo {
			get { return MemberInfo; }
		}

		object IValidationError.ContextObject {
			get { return ContextObject; }
		}
	}
}
