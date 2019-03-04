using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer.Photon
{
	internal class BattleCancelledEventListener : LobbyEventListener, IBattleCancelledEventListener, IServiceEventListener, IServiceEventListenerBase
	{
		public override LobbyEventCode LobbyEventCode => LobbyEventCode.BattleCancelled;

		protected override void ParseData(EventData eventData)
		{
			Invoke();
		}
	}
}
