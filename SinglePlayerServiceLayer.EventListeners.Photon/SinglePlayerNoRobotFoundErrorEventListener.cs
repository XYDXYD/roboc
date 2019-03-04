using ExitGames.Client.Photon;
using SinglePlayerServiceLayer.Photon;
using Svelto.ServiceLayer;

namespace SinglePlayerServiceLayer.EventListeners.Photon
{
	internal class SinglePlayerNoRobotFoundErrorEventListener : SinglePlayerEventListener<string>, ISinglePlayerNoRobotFoundErrorEventListener, IServiceEventListener<string>, IServiceEventListenerBase
	{
		public override SinglePlayerEventCode SinglePlayerEventCode => SinglePlayerEventCode.NoRobotFoundError;

		protected override void ParseData(EventData eventData)
		{
			string empty = string.Empty;
			Invoke(empty);
		}
	}
}
