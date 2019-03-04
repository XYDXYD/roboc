using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace LobbyServiceLayer.Photon
{
	internal class SendPlatoonUpdateRequest : LobbyRequest, ISendPlatoonUpdateRequest, IServiceRequest<Platoon>, IServiceRequest
	{
		private Platoon _platoon;

		protected override byte OperationCode => 2;

		public SendPlatoonUpdateRequest()
			: base("strLobbyError", "strErrorUpdatingLobby", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(2, _platoon.platoonId);
			dictionary.Add(4, _platoon.Size);
			Dictionary<byte, object> dictionary2 = val.Parameters = dictionary;
			return val;
		}

		public void Inject(Platoon platoon)
		{
			_platoon = platoon;
		}

		public override void Execute()
		{
			PhotonLobbyUtility.Instance.PlatoonUpdate(_platoon);
			base.Execute();
		}
	}
}
