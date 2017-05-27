using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTValidator.Internal {
	public static class GameObjectExtensions {
		public static IEnumerable<GameObject> GetChildren(this GameObject g) {
			foreach (Transform childTransform in g.transform) {
				yield return childTransform.gameObject;
			}
		}

		public static GameObject GetParent(this GameObject g) {
			if (g.transform.parent == null) {
				return null;
			}

			return g.transform.parent.gameObject;
		}

		public static IEnumerable<GameObject> GetParents(this GameObject g) {
			while (g != null) {
				yield return g.GetParent();

				g = g.GetParent();
			}
		}
	}
}
