using ExitGames.Client.Photon;
using System;

namespace Services
{
	public class BrawlClientParameters
	{
		public readonly bool IsLocked;

		public readonly int VersionNumber;

		public readonly int BrawlNumber;

		public BrawlClientParameters(bool isLocked_, int versionNumber_, int brawlNumber_)
		{
			IsLocked = isLocked_;
			VersionNumber = versionNumber_;
			BrawlNumber = brawlNumber_;
		}

		public static BrawlClientParameters Deserialise(Hashtable data)
		{
			bool isLocked_ = Convert.ToBoolean(data.get_Item((object)"IsLocked"));
			int versionNumber_ = Convert.ToInt32(data.get_Item((object)"CurrentVersionNumber"));
			int brawlNumber_ = Convert.ToInt32(data.get_Item((object)"BrawlNumber"));
			return new BrawlClientParameters(isLocked_, versionNumber_, brawlNumber_);
		}
	}
}
