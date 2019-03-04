using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Utility;

namespace Services.Web.Photon
{
	internal class CustomGameGetTeamSetupRequest : WebServicesCachedRequest<int>, ICustomGameGetTeamSetupRequest, IServiceRequest<GameModeType>, IAnswerOnComplete<int>, IServiceRequest
	{
		private const int DEFAULT_TEAM_SIZE = 5;

		private GameModeType _gameMode;

		protected override byte OperationCode => 162;

		public CustomGameGetTeamSetupRequest()
			: base("strRobocloudError", "strCustomGameGetTeamSetupError", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override int ProcessResponse(OperationResponse response)
		{
			int result = 5;
			try
			{
				Dictionary<string, int> dictionary = (Dictionary<string, int>)response.Parameters[168];
				result = dictionary[_gameMode.ToString()];
				return result;
			}
			catch (Exception)
			{
				Console.LogError("CustomGameGetTeamSetupRequest error: failed to retrieve expected team size, defaulting to 5.");
				return result;
			}
		}

		public void Inject(GameModeType dependency)
		{
			_gameMode = dependency;
		}
	}
}
