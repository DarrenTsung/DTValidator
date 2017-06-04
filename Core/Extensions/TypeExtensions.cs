using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DTValidator.Internal {
	public static class TypeExtensions {
		public static FieldInfo GetRequiredField(this Type type, string name) {
			FieldInfo fieldInfo = type.GetField(name);
			if (fieldInfo == null) {
				Debug.LogError(string.Format("Failed to find field named: '{0}' on type: {1}", name, type.Name));
			}
			return fieldInfo;
		}

		public static PropertyInfo GetRequiredProperty(this Type type, string name) {
			PropertyInfo propertyInfo = type.GetProperty(name);
			if (propertyInfo == null) {
				Debug.LogError(string.Format("Failed to find property named: '{0}' on type: {1}", name, type.Name));
			}
			return propertyInfo;
		}
	}
}
