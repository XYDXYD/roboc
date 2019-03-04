using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Utility;

namespace LobbyServiceLayer.Photon
{
	internal class ErrorJoiningQueueEventListener : LobbyEventListener<LobbyReasonCode>, IErrorJoiningQueueEventListener, IServiceEventListener<LobbyReasonCode>, IServiceEventListenerBase
	{
		public override LobbyEventCode LobbyEventCode => LobbyEventCode.ErrorJoiningQueue;

		protected override void ParseData(EventData eventData)
		{
			short data = (short)eventData.Parameters[21];
			string text = (string)eventData.Parameters[22];
			RemoteLogger.Error("Error reported joining matchmaking queue", text, null);
			Console.LogError(text);
			Invoke((LobbyReasonCode)data);
			PhotonLobbyUtility.Instance.Disconnect();
		}
	}
}
