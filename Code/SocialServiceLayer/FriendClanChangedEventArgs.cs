namespace SocialServiceLayer
{
	internal struct FriendClanChangedEventArgs
	{
		public readonly string userName;

		public readonly string clanName;

		public FriendClanChangedEventArgs(string userName, string clanName)
		{
			this.userName = userName;
			this.clanName = clanName;
		}
	}
}
