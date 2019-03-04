using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class PollClanExperienceRequestResponse
	{
		public readonly Dictionary<string, int> clanSeasonXPvalues;

		public PollClanExperienceRequestResponse(Dictionary<string, int> clanXpValues_)
		{
			clanSeasonXPvalues = clanXpValues_;
		}
	}
}
