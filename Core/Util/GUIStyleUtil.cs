#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DTValidator {
	public static class GUIStyleUtil {
		public static GUIStyle StyleWithBackgroundColor(Color color) {
			return StyleWithTexture(Texture2DUtil.GetCached1x1TextureWithColor(color));
		}

		public static GUIStyle StyleWithTexture(Texture2D texture) {
			return StyleWithTexture(GUIStyle.none, texture);
		}

		public static GUIStyle StyleWithTexture(GUIStyle baseStyle, Texture2D texture) {
			GUIStyle style = cachedTextureStyles_.GetValueOrDefault(texture);
			if (style == null) {
				style = new GUIStyle(baseStyle);
				style.normal.background = texture;
			}

			return style;
		}


		// PRAGMA MARK - Internal
		private static Dictionary<Texture2D, GUIStyle> cachedTextureStyles_ = new Dictionary<Texture2D, GUIStyle>();
	}
}
#endif