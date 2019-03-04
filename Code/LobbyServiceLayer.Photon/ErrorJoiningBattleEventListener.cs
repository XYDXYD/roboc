using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Utility;

namespace LobbyServiceLayer.Photon
{
	internal class ErrorJoiningBattleEventListener : LobbyEventListener<LobbyReasonCode>, IErrorJoiningBattleEventListener, IServiceEventListener<LobbyReasonCode>, IServiceEventListenerBase
	{
		public override LobbyEventCode LobbyEventCode => LobbyEventCode.ErrorJoiningBattle;

		protected override void ParseData(EventData eventData)
		{
			short data = (short)eventData.Parameters[21];
			string text = (string)eventData.Parameters[22];
			RemoteLogger.Error("Error reported joining battle", text, null);
			Console.LogError(text);
			Invoke((LobbyReasonCode)data);
			PhotonLobbyUtility.Instance.Disconnect();
		}
	}
}
