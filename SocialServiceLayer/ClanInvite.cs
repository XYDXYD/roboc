namespace SocialServiceLayer
{
	internal class ClanInvite
	{
		public readonly string InviterName;

		public readonly string InviterDisplayName;

		public readonly string ClanName;

		public readonly int ClanSize;

		public readonly AvatarInfo InviterAvatarInfo;

		public ClanInvite(string inviterName, string inviterDisplayName, string clanName, int clanSize, AvatarInfo inviterAvatarInfo)
		{
			InviterName = inviterName;
			InviterDisplayName = inviterDisplayName;
			ClanName = clanName;
			ClanSize = clanSize;
			InviterAvatarInfo = inviterAvatarInfo;
		}
	}
}
