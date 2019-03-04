using Authentication;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using System;
using Utility;

namespace SinglePlayerServiceLayer.Photon
{
	internal class SinglePlayerClient : PhotonClient
	{
		protected override string serverAddressS3Key => "PhotonSinglePlayerServer";

		protected override string serverAddressLocalOverrideKey => "SinglePlayerServerAddress";

		protected override string applicationID => "SinglePlayerServer";

		protected override string appVersion => "v1";

		protected override int serviceConnectionInterval => 50;

		protected override int pingInterval => 3000;

		protected override int connectAttempts => 1;

		protected override float connectAttemptDelay => 1f;

		protected override byte duplicateLoginCode => 0;

		protected override byte ccuExceededCode => 1;

		protected override byte ccuCheckPassedCode => 2;

		protected override bool CCUCheckRequired => false;

		protected override byte maxPlayerPerRoom => 50;

		protected override int emptyRoomTtl => 10000;

		public SinglePlayerClient()
			: base(1)
		{
		}

		public override void OnOperationResponse(OperationResponse operationResponse)
		{
			if (operationResponse.ReturnCode == 3)
			{
				DisconnectWithError(new Exception("Wrong number of authentication params sent to webservices server. Please check your game version"));
			}
			else
			{
				base.OnOperationResponse(operationResponse);
			}
		}

		protected override string GetAuthenticationParameters()
		{
			return base.GetAuthenticationParameters() + "," + User.SessionId;
		}

		protected override void OnConnectedToMaster()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			Console.Log("Photon web services single player peer connected to master server");
			RoomOptions val = new RoomOptions();
			if (this.OpJoinOrCreateRoom(UserName + "webservices", val, null, (string[])null))
			{
				Console.Log("Joining photon web services single player room...");
			}
		}
	}
}
