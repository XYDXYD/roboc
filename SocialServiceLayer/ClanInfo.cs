using ExitGames.Client.Photon;

namespace SocialServiceLayer
{
	public class ClanInfo
	{
		public string ClanName;

		public string ClanDescription
		{
			get;
			set;
		}

		public ClanType ClanType
		{
			get;
			set;
		}

		public int ClanSize
		{
			get;
			set;
		}

		public int ClanAvatarNumber
		{
			get;
			set;
		}

		public ClanInfo()
		{
		}

		internal ClanInfo(string clanName, string clanDescription, ClanType clanType, int clanSize = 1)
		{
			ClanName = clanName;
			ClanDescription = clanDescription;
			ClanType = clanType;
			ClanSize = clanSize;
		}

		public static ClanInfo FromHashtable(Hashtable clanInfoRaw)
		{
			ClanInfo clanInfo = new ClanInfo((string)clanInfoRaw.get_Item((object)"clanName"), (string)clanInfoRaw.get_Item((object)"clanDescription"), (ClanType)clanInfoRaw.get_Item((object)"clanType"), (int)clanInfoRaw.get_Item((object)"clanSize"));
			clanInfo.ClanAvatarNumber = (int)clanInfoRaw.get_Item((object)"clanAvatarNumber");
			return clanInfo;
		}
	}
}
