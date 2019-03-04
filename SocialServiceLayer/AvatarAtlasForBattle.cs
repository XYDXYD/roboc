using System.Collections.Generic;
using UnityEngine;

namespace SocialServiceLayer
{
	internal class AvatarAtlasForBattle
	{
		public readonly Texture2D AtlasTexture;

		public readonly IDictionary<string, Rect> AtlasRects;

		public AvatarAtlasForBattle(Texture2D atlasTexture, IDictionary<string, Rect> atlasRects)
		{
			AtlasTexture = atlasTexture;
			AtlasRects = atlasRects;
		}
	}
}
