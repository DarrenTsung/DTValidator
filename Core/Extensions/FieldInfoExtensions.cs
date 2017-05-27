using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DTValidator.Internal {
	public static class FieldInfoExtensions {
		public static IEnumerable<UnityEngine.Object> GetUnityEngineObjects(this FieldInfo fieldInfo, object obj) {
			if (fieldInfo.FieldType.IsClass && typeof(UnityEngine.Object).IsAssignableFrom(fieldInfo.FieldType)) {
				yield return (UnityEngine.Object)fieldInfo.GetValue(obj);
			} else if (typeof(IEnumerable).IsAssignableFrom(fieldInfo.FieldType)) {
				var enumerable = (IEnumerable)fieldInfo.GetValue(obj);
				foreach (var o in enumerable.OfType<UnityEngine.Object>()) {
					yield return o;
				}
			}
		}
	}
}
