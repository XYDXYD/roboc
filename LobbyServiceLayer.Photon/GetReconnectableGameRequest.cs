using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace LobbyServiceLayer.Photon
{
	internal class GetReconnectableGameRequest : LobbyRequest<EnterBattleDependency>, IGetReconnectableGameRequest, IServiceRequest, IAnswerOnComplete<EnterBattleDependency>
	{
		protected override byte OperationCode => 8;

		public GetReconnectableGameRequest()
			: base("strRobocloudError", "strCannotGetReconnectableGame", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override EnterBattleDependency ProcessResponse(OperationResponse response)
		{
			if (!response.Parameters.ContainsKey(6))
			{
				return null;
			}
			EnterBattleDependency enterBattleDependency = EnterBattleEventListener.ParseData(response.Parameters, DateTime.UtcNow);
			EnterBattleEventListener.CacheData(enterBattleDependency);
			return enterBattleDependency;
		}
	}
}
