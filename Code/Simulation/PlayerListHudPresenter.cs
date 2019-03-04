using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class PlayerListHudPresenter
	{
		public Texture2D AvatarAtlasTexture
		{
			get;
			private set;
		}

		public IDictionary<string, Rect> AvatarAtlasRects
		{
			get;
			private set;
		}

		public Texture2D ClanAvatarAtlasTexture
		{
			get;
			private set;
		}

		public IDictionary<string, Rect> ClanAvatarAtlasRects
		{
			get;
			private set;
		}

		internal void InjectMultiplayerAvatars(Texture2D avatarAtlasTexture, IDictionary<string, Rect> avatarAtlasRects, Texture2D clanAvatarAtlas, IDictionary<string, Rect> clanAvatarAtlasRects)
		{
			AvatarAtlasTexture = avatarAtlasTexture;
			AvatarAtlasRects = avatarAtlasRects;
			ClanAvatarAtlasTexture = clanAvatarAtlas;
			ClanAvatarAtlasRects = clanAvatarAtlasRects;
		}
	}
}
