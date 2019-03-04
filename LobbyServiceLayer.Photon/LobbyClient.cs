using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using System;
using Utility;

namespace LobbyServiceLayer.Photon
{
	internal class LobbyClient : PhotonClient
	{
		private Platoon _platoon;

		protected override string serverAddressS3Key => "PhotonLobbyServer";

		protected override string serverAddressLocalOverrideKey => "LobbyServerAddress";

		protected override string applicationID => "LobbyServer";

		protected override string appVersion => "v1";

		protected override int serviceConnectionInterval => 100;

		protected override int pingInterval => 3000;

		protected override int connectAttempts => 1;

		protected override float connectAttemptDelay => 0f;

		protected override byte duplicateLoginCode => 0;

		protected override byte ccuExceededCode => 40;

		protected override byte ccuCheckPassedCode => 41;

		protected override bool CCUCheckRequired => false;

		protected override byte maxPlayerPerRoom => 50;

		protected override int emptyRoomTtl => 10000;

		public LobbyClient()
			: base(1)
		{
		}

		public void PlatoonUpdate(Platoon platoon)
		{
			_platoon = platoon;
		}

		protected override void OnConnectedToMaster()
		{
			Console.Log("Photon lobby peer connected to master server");
			JoinLobbyRoom();
		}

		private void JoinLobbyRoom()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			string text = (_platoon == null || !_platoon.isInPlatoon) ? UserName : _platoon.platoonId;
			this.OpJoinOrCreateRoom(text, new RoomOptions(), null, (string[])null);
		}

		public override void OnEvent(EventData eventData)
		{
			LobbyEventCode lobbyEventCode = (LobbyEventCode)Enum.ToObject(typeof(LobbyEventCode), eventData.Code);
			Console.Log($"Received lobby event with code {eventData.Code} which translates to {lobbyEventCode}");
			base.OnEvent(eventData);
		}
	}
}
