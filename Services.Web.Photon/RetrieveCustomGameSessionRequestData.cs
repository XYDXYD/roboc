using CustomGames;

namespace Services.Web.Photon
{
	internal class RetrieveCustomGameSessionRequestData
	{
		public readonly CustomGameSessionData Data;

		public readonly CustomGameSessionRetrieveResponse Response;

		public RetrieveCustomGameSessionRequestData(CustomGameSessionData sessionData_, CustomGameSessionRetrieveResponse response_)
		{
			Data = sessionData_;
			Response = response_;
		}
	}
}
