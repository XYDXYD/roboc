using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace LobbyServiceLayer.Photon
{
	internal class ConnectionTestResultRequest : LobbyRequest, IConnectionTestResultRequest, IServiceRequest<bool>, IAnswerOnComplete, IServiceRequest
	{
		private bool _success;

		protected override byte OperationCode => 4;

		public ConnectionTestResultRequest()
			: base("strLobbyError", "strLobbyJoinFail", 0)
		{
		}

		public void Inject(bool success)
		{
			_success = success;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, _success);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = parameters;
			return val;
		}
	}
}
