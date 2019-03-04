namespace SocialServiceLayer.Photon
{
	internal class CreateClanRequestDependancyTencent : CreateClanRequestDependancyBase
	{
		public int clanDefaultAvatar;

		public CreateClanRequestDependancyTencent(string clanName_, string clanDescription_, ClanType clanType_, int clanDefaultAvatar_)
			: base(clanName_, clanDescription_, clanType_)
		{
			clanDefaultAvatar = clanDefaultAvatar_;
		}
	}
}
