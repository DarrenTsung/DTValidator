using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTValidator.Internal {
	public static class ValidatorIgnoredNamespaceProvider {
		public static IEnumerable<ValidatorIgnoredNamespace> GetIgnoredNamespaces() {
			if (currentProvider_ == null) {
				return AssetDatabaseUtil.AllAssetsOfType<ValidatorIgnoredNamespace>();
			}

			return currentProvider_.Invoke();
		}

		public static void SetCurrentProvider(Func<IEnumerable<ValidatorIgnoredNamespace>> provider) {
			currentProvider_ = provider;
		}

		public static void ClearCurrentProvider() {
			currentProvider_ = null;
		}


		// PRAGMA MARK - Internal
		private static Func<IEnumerable<ValidatorIgnoredNamespace>> currentProvider_;
	}
}