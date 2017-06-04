#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

using DTValidator.Internal;

namespace DTValidator {
	public static class ValidatorUnityWhitelist {
		// PRAGMA MARK - Static Public Interface
		// NOTE (darren): MeshFilter doesn't have mesh as a field, it has it as a property
		public static readonly MemberInfo kMeshFilterSharedMesh = typeof(UnityEngine.MeshFilter).GetRequiredProperty("sharedMesh");

		public static bool IsTypeWhitelisted(Type type) {
			return kTypes.ContainsKey(type);
		}

		public static HashSet<MemberInfo> GetWhitelistedMembersFor(Type type) {
			return kTypes[type];
		}

		public static IEnumerable<Predicate<object>> GetOptionalPredicatesFor(MemberInfo memberInfo) {
			return kMemberInfoPredicates.GetValueOrDefault(memberInfo);
		}

		public static void RegisterPredicateFor(MemberInfo memberInfo, Predicate<object> predicate) {
			var predicates = kMemberInfoPredicates.GetAndCreateIfNotFound(memberInfo);
			predicates.Add(predicate);
		}

		public static void UnregisterPredicateFor(MemberInfo memberInfo, Predicate<object> predicate) {
			var predicates = kMemberInfoPredicates.GetValueOrDefault(memberInfo);
			if (predicates == null) {
				Debug.LogWarning("UnregisterPredicateFor - failed because no predicates registered for: " + memberInfo);
				return;
			}

			predicates.Remove(predicate);
		}


		// PRAGMA MARK - Internal
		private static readonly Dictionary<Type, HashSet<MemberInfo>> kTypes = new Dictionary<Type, HashSet<MemberInfo>>() {
			{
				typeof(UnityEngine.MeshFilter), new HashSet<MemberInfo>()
				{
					kMeshFilterSharedMesh
				}
			},
		};

		private static readonly Dictionary<MemberInfo, HashSet<Predicate<object>>> kMemberInfoPredicates = new Dictionary<MemberInfo, HashSet<Predicate<object>>>() {
			{ kMeshFilterSharedMesh, new HashSet<Predicate<object>>() { DontValidateIfTextMeshPro } }
		};

		private static readonly Type kTextMeshProFirstPassType = Type.GetType("TMPro.TextMeshPro, Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
		private static readonly Type kTextMeshProType = Type.GetType("TMPro.TextMeshPro, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

		// NOTE (darren): we want to validate and check MeshFilters, but
		// TextMeshPro dynamically adds a mesh. As a requirement to validation,
		// MeshFilter must not have a TextMeshPro component as well
		private static bool DontValidateIfTextMeshPro(object obj) {
			UnityEngine.Component component = obj as UnityEngine.Component;
			if (component == null) {
				return true;
			}

			Type textMeshProType = kTextMeshProType ?? kTextMeshProFirstPassType;
			if (textMeshProType == null) {
				return true;
			}

			UnityEngine.Component textMeshProComponent = component.GetComponent(textMeshProType);
			// valid (true) when textMeshProComponent does not exist
			return textMeshProComponent == null;
		}
	}
}
#endif
