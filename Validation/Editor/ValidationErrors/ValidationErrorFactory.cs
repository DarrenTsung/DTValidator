using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace DTValidator.Internal {
	public static class ValidationErrorFactory {
		// PRAGMA MARK - Static Public Interface
		public static IValidationError Create(object obj, Type objectType, MemberInfo memberInfo, object contextObject) {
			Component objAsComponent = obj as Component;
			if (objAsComponent != null) {
				return new ComponentValidationError(objAsComponent, objectType, memberInfo, contextObject);
			} else {
				return new ObjectValidationError(obj, objectType, memberInfo, contextObject);
			}
		}

		public static IValidationError Create(object obj, Type objectType, MemberInfo memberInfo, object contextObject, int index) {
			return new IndexedObjectValidationError(obj, objectType, memberInfo, contextObject, index);
		}
	}
}
