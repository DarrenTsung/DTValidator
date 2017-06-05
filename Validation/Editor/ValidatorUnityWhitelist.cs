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

		public static void RegisterWhitelistedTypeMember(Type type, MemberInfo memberInfo) {
			HashSet<MemberInfo> members = kTypes.GetAndCreateIfNotFound(type);
			members.Add(memberInfo);
		}

		public static void UnregisterWhitelistedTypeMember(Type type, MemberInfo memberInfo) {
			HashSet<MemberInfo> members = kTypes.GetAndCreateIfNotFound(type);
			members.Remove(memberInfo);
		}

		public static PropertyInfo GetPropertyFrom(Type type, string propertyName) {
			return type.GetRequiredProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		}

		public static FieldInfo GetFieldFrom(Type type, string fieldName) {
			return type.GetRequiredField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		}


		// PRAGMA MARK - Internal
		private static readonly Dictionary<Type, HashSet<MemberInfo>> kTypes = new Dictionary<Type, HashSet<MemberInfo>>();
		private static readonly Dictionary<MemberInfo, HashSet<Predicate<object>>> kMemberInfoPredicates = new Dictionary<MemberInfo, HashSet<Predicate<object>>>();
	}
}
#endif
