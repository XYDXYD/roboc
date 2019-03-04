using ExitGames.Client.Photon;
using SinglePlayerServiceLayer.Photon;
using SinglePlayerServiceLayer.Requests.Photon;
using Svelto.ServiceLayer;

namespace SinglePlayerServiceLayer.EventListeners.Photon
{
	internal class SinglePlayerSpawnRobotEventListener : SinglePlayerEventListener<CommunityRobotData>, ISinglePlayerSpawnRobotEventListener, IServiceEventListener<CommunityRobotData>, IServiceEventListenerBase
	{
		public override SinglePlayerEventCode SinglePlayerEventCode => SinglePlayerEventCode.SpawnRobot;

		protected override void ParseData(EventData eventData)
		{
			string robotGuid = (string)eventData.Parameters[2];
			byte[] machineModel = (byte[])eventData.Parameters[3];
			string robotName = (string)eventData.Parameters[4];
			byte[] colorModel = (byte[])eventData.Parameters[7];
			CommunityRobotData data = new CommunityRobotData(robotGuid, machineModel, colorModel, robotName);
			Invoke(data);
		}
	}
}
