namespace SocialServiceLayer.Photon
{
	internal class CreateClanRequestDependancyBase
	{
		public string clanName;

		public string clanDescription;

		public ClanType clanType;

		public CreateClanRequestDependancyBase(string clanName_, string clanDescription_, ClanType clanType_)
		{
			clanName = clanName_;
			clanDescription = clanDescription_;
			clanType = clanType_;
		}
	}
}
