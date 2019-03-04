using System;
using System.Collections.Generic;
using UnityEngine;

namespace Avatars
{
	internal class MultiAvatarCache
	{
		private Dictionary<string, Texture2D> PlayerAvatars;

		private Dictionary<string, Texture2D> ClanAvatars;

		public MultiAvatarCache()
		{
			PlayerAvatars = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);
			ClanAvatars = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);
		}

		public void SetAvatar(AvatarType avatarType, string name, Texture2D texture)
		{
			if (avatarType == AvatarType.PlayerAvatar)
			{
				PlayerAvatars[name] = texture;
			}
			else
			{
				ClanAvatars[name] = texture;
			}
		}

		public Texture2D GetAvatar(AvatarType avatarType, string name)
		{
			if (avatarType == AvatarType.PlayerAvatar)
			{
				if (PlayerAvatars.ContainsKey(name))
				{
					return PlayerAvatars[name];
				}
				return null;
			}
			if (ClanAvatars.ContainsKey(name))
			{
				return ClanAvatars[name];
			}
			return null;
		}

		public void ClearPlayerAvatar(string name)
		{
			PlayerAvatars.Remove(name);
		}

		public void ClearClanAvatar(string name)
		{
			ClanAvatars.Remove(name);
		}

		public void ClearCache()
		{
			PlayerAvatars.Clear();
			ClanAvatars.Clear();
		}
	}
}
