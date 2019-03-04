using ExitGames.Client.Photon;
using SinglePlayerServiceLayer.Photon;
using Svelto.ServiceLayer;

namespace SinglePlayerServiceLayer.EventListeners.Photon
{
	internal class SinglePlayerUpdateExperienceEventListener : SinglePlayerEventListener<int>, ISinglePlayerUpdateExperienceEventListener, IServiceEventListener<int>, IServiceEventListenerBase
	{
		public override SinglePlayerEventCode SinglePlayerEventCode => SinglePlayerEventCode.UpdateExperience;

		protected override void ParseData(EventData eventData)
		{
			int data = (int)eventData.Parameters[6];
			Invoke(data);
		}
	}
}
