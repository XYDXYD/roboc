using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal class GetGameModePreferencesRequest : WebServicesCachedRequest<GameModePreferences>, IGetGameModePreferencesRequest, IServiceRequest, IAnswerOnComplete<GameModePreferences>
	{
		protected override byte OperationCode => 76;

		public GetGameModePreferencesRequest()
			: base("strRobocloudError", "strUnableToGetGameModePreferencesError", 1)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override GameModePreferences ProcessResponse(OperationResponse response)
		{
			object obj = response.Parameters[215];
			return new GameModePreferences((int)obj);
		}

		void IGetGameModePreferencesRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
