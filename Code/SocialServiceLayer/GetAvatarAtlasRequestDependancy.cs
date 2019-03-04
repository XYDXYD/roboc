using System.Collections.Generic;
using UnityEngine;

namespace SocialServiceLayer
{
	internal class GetAvatarAtlasRequestDependancy
	{
		public IDictionary<string, AvatarInfo> avatarInfos;

		public Dictionary<string, Texture2D> loadedCustomAvatarTextures;

		public GetAvatarAtlasRequestDependancy(IDictionary<string, AvatarInfo> avatarInfos_, Dictionary<string, Texture2D> loadedCustomAvatarTextures_)
		{
			avatarInfos = avatarInfos_;
			loadedCustomAvatarTextures = loadedCustomAvatarTextures_;
		}
	}
}
