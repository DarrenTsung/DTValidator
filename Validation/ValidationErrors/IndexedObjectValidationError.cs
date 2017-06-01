using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace DTValidator.Internal {
	public class IndexedObjectValidationError : IValidationError {
		// PRAGMA MARK - Public Interface
		public readonly object Object;
		public readonly Type ObjectType;
		public readonly FieldInfo FieldInfo;
		public readonly object ContextObject;
		public readonly int Index;

		public IndexedObjectValidationError(object obj, Type objectType, FieldInfo fieldInfo, object contextObject, int index) {
			Object = obj;
			ObjectType = objectType;
			FieldInfo = fieldInfo;
			ContextObject = contextObject;
			Index = index;
		}

		public override string ToString() {
			return string.Format("IOVE (Object: {0}->{1}[{2}]) context: {3}", (Object as UnityEngine.Object).name, FieldInfo.Name, Index, ContextObject);
		}


		// PRAGMA MARK - IValidationError Implementation
		object IValidationError.Object {
			get { return Object; }
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
