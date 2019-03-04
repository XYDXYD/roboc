using ExitGames.Client.Photon;
using LobbyServiceLayer.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;
using Utility;

namespace LobbyServiceLayer
{
	internal class UnregisterPlayerFromReconnectableGameClientRequest : LobbyRequest, IUnregisterPlayerFromReconnectableGameClientRequest, IServiceRequest, IAnswerOnComplete
	{
		protected override byte OperationCode => 9;

		public UnregisterPlayerFromReconnectableGameClientRequest()
			: base("strRobocloudError", "strUnregisterPlayerFromReconnectableGame", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			Console.Log("Send player unregister from Reconnect");
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			return val;
		}
	}
}
