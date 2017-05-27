using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace DTValidator.Internal {
	public class ObjectValidationError : IValidationError {
		// PRAGMA MARK - Public Interface
		public readonly object Object;
		public readonly Type ObjectType;
		public readonly FieldInfo FieldInfo;

		public ObjectValidationError(object obj, Type objectType, FieldInfo fieldInfo) {
			Object = obj;
			ObjectType = objectType;
			FieldInfo = fieldInfo;
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
	}
}
