using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer.Photon
{
	internal class ErrorInQueueEventListener : LobbyEventListener<LobbyReasonCode>, IErrorInQueueEventListener, IServiceEventListener<LobbyReasonCode>, IServiceEventListenerBase
	{
		public override LobbyEventCode LobbyEventCode => LobbyEventCode.ErrorInQueue;

		protected override void ParseData(EventData eventData)
		{
			short data = (short)eventData.Parameters[21];
			string details = (string)eventData.Parameters[22];
			RemoteLogger.Error("Error reported in matchmaking queue", details, null);
			Invoke((LobbyReasonCode)data);
			PhotonLobbyUtility.Instance.Disconnect();
		}
	}
}
