using System.Collections.Generic;
using UnityEngine;

namespace SocialServiceLayer
{
	internal class GetClanAvatarAtlasRequestDependancy
	{
		public Dictionary<string, string> clanByPlayer;

		public Dictionary<string, Texture2D> loadedClanAvatarTextures;

		public GetClanAvatarAtlasRequestDependancy(Dictionary<string, Texture2D> loadedClanAvatarTextures_, Dictionary<string, string> clanByPlayer_)
		{
			loadedClanAvatarTextures = loadedClanAvatarTextures_;
			clanByPlayer = clanByPlayer_;
		}
	}
}
