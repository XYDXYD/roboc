namespace Mothership.GUI.Party
{
	internal struct InvitablePlayerData
	{
		public readonly string playerName;

		public readonly string displayName;

		public readonly bool invitable;

		public readonly AvatarInfo avatarInfo;

		public InvitablePlayerData(string pName, string dName, bool pInvitable, AvatarInfo pAvatarInfo)
		{
			playerName = pName;
			displayName = dName;
			invitable = pInvitable;
			avatarInfo = pAvatarInfo;
		}
	}
}
