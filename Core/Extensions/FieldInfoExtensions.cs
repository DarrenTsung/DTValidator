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
				if (fieldInfo.FieldType.IsGenericType && (typeof(List<>)).IsAssignableFrom(fieldInfo.FieldType.GetGenericTypeDefinition())) {
					if (!typeof(UnityEngine.Object).IsAssignableFrom(fieldInfo.FieldType.GetGenericArguments()[0])) {
						yield break;
					}
				} else {
					if (!typeof(UnityEngine.Object).IsAssignableFrom(fieldInfo.FieldType.GetElementType())) {
						yield break;
					}
				}

				var enumerable = (IEnumerable)fieldInfo.GetValue(obj);
				if (enumerable == null) {
					// NOTE (darren): it's possible for a serialized enumerable like int[] to be
					// null instead of empty enumerable - there is nothing to iterate over
					yield break;
				}

				foreach (var o in enumerable) {
					yield return o as UnityEngine.Object;
				}
			}
		}
	}
}
