using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer.Photon
{
	internal class BattleFoundEventListener : LobbyEventListener, IBattleFoundEventListener, IServiceEventListener, IServiceEventListenerBase
	{
		public override LobbyEventCode LobbyEventCode => LobbyEventCode.BattleFound;

		protected override void ParseData(EventData eventData)
		{
			Invoke();
		}
	}
}
