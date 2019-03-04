namespace Mothership.GUI.CustomGames
{
	internal class PartyIconTierChangeMessage
	{
		public readonly int NewTier;

		public readonly string UserName;

		public readonly string TierTextString;

		public PartyIconTierChangeMessage(string username_, int newtier_, string tierTextString_)
		{
			NewTier = newtier_;
			UserName = username_;
			TierTextString = tierTextString_;
		}

		public PartyIconTierChangeMessage(string username_)
		{
			UserName = username_;
			TierTextString = string.Empty;
			NewTier = 0;
		}
	}
}
