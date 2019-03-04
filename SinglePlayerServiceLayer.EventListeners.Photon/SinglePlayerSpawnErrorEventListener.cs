using ExitGames.Client.Photon;
using SinglePlayerServiceLayer.Photon;
using Svelto.ServiceLayer;

namespace SinglePlayerServiceLayer.EventListeners.Photon
{
	internal class SinglePlayerSpawnErrorEventListener : SinglePlayerEventListener<string>, ISinglePlayerSpawnErrorEventListener, IServiceEventListener<string>, IServiceEventListenerBase
	{
		public override SinglePlayerEventCode SinglePlayerEventCode => SinglePlayerEventCode.SpawnRobotError;

		protected override void ParseData(EventData eventData)
		{
			string empty = string.Empty;
			Invoke(empty);
		}
	}
}
