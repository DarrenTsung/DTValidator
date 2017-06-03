using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace DTValidator.Internal {
	public class ObjectValidationError : IValidationError {
		// PRAGMA MARK - Public Interface
		public readonly int ObjectLocalId;
		public readonly Type ObjectType;
		public readonly object ContextObject;
		public readonly FieldInfo FieldInfo;

		public ObjectValidationError(object obj, Type objectType, FieldInfo fieldInfo, object contextObject) {
			ObjectLocalId = (obj as UnityEngine.Object).GetLocalId();
			ObjectType = objectType;
			FieldInfo = fieldInfo;
			ContextObject = contextObject;
		}

		public override string ToString() {
			return string.Format("OVE ({0}->{1}) context: {2}", FieldInfo.DeclaringType.Name, FieldInfo.Name, ContextObject);
		}


		// PRAGMA MARK - IValidationError Implementation
		int IValidationError.ObjectLocalId {
			get { return ObjectLocalId; }
		}

		Type IValidationError.ObjectType {
			get { return ObjectType; }
		}

		FieldInfo IValidationError.FieldInfo {
			get { return FieldInfo; }
		}

		object IValidationError.ContextObject {
			get { return ContextObject; }
		}
	}
}
