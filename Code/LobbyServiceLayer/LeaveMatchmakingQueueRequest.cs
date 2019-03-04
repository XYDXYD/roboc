using LobbyServiceLayer.Photon;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal class LeaveMatchmakingQueueRequest : ILeaveMatchmakingQueueRequest, IServiceRequest
	{
		public void Execute()
		{
			PhotonLobbyUtility.Instance.Disconnect();
		}
	}
}
