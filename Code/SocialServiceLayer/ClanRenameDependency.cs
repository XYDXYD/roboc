namespace SocialServiceLayer
{
	internal struct ClanRenameDependency
	{
		public readonly string OldClanName;

		public readonly string NewClanName;

		public readonly string AdminName;

		public ClanRenameDependency(string oldClanName, string newClanName, string adminName)
		{
			this = default(ClanRenameDependency);
			OldClanName = oldClanName;
			NewClanName = newClanName;
			AdminName = adminName;
		}
	}
}
