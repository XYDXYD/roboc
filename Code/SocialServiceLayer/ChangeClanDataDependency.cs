namespace SocialServiceLayer
{
	internal class ChangeClanDataDependency
	{
		public string ClanName
		{
			get;
			set;
		}

		public string NewDescription
		{
			get;
			set;
		}

		public ClanType? NewType
		{
			get;
			set;
		}

		public int? NewDefaultClanAvatarId
		{
			get;
			set;
		}
	}
}
