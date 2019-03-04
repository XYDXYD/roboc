using UnityEngine;

namespace Mothership
{
	internal class FriendListEntryData
	{
		public enum FriendListButtonTypes
		{
			NONE,
			TICK,
			CROSS,
			PLUS
		}

		public string StatusText;

		public int StatusColourIndex;

		public FriendListButtonTypes LeftButton;

		public FriendListButtonTypes RightButton;

		public Texture2D FriendsPlayerAvatar;

		public Friend Friend;

		public string FriendName => Friend.Name;

		public string DisplayName => Friend.DisplayName;

		public string ClanName => Friend.ClanName;

		public AvatarInfo Avatar => Friend.AvatarInfo;
	}
}
