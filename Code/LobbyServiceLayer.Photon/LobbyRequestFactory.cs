using SocialServiceLayer;
using Svelto.DataStructures;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer.Photon
{
	internal class LobbyRequestFactory : ServiceRequestFactory, ILobbyRequestFactory, IServiceRequestFactory
	{
		public LobbyRequestFactory()
		{
			AddRelation<IRetrieveExpectedPlayersListRequest, RetrieveExpectedPlayersListRequest>();
			AddRelation<IGetMyAndEnemyTeamRequest, GetMyAndEnemyTeamRequest>();
			AddRelation<IGetBattleParametersRequest, GetBattleParametersRequest>();
			AddRelation<IEnterMatchmakingQueueRequest, EnterMatchmakingQueueRequest, EnterMatchmakingQueueDependency>();
			AddRelation<ILeaveMatchmakingQueueRequest, LeaveMatchmakingQueueRequest>();
			AddRelation<ISetExpectedPlayerRequest, SetExpectedPlayersRequestRequest, ReadOnlyDictionary<string, PlayerDataDependency>>();
			AddRelation<ISendPlatoonUpdateRequest, SendPlatoonUpdateRequest, Platoon>();
			AddRelation<IGetQuitterPenaltyInfo, GetQuitterPenaltyInfo>();
			AddRelation<IGetMultilayerAvatarInfoRequest, GetMultilayerAvatarInfoRequest>();
			AddRelation<ISetMultilayerAvatarInfoRequest, SetMultilayerAvatarInfoRequest, ReadOnlyDictionary<string, AvatarInfo>>();
			AddRelation<IConnectionTestResultRequest, ConnectionTestResultRequest>();
			AddRelation<IGetClanInfosRequest, GetClanInfosRequest>();
			AddRelation<ISetClanInfosRequest, SetClanInfosRequest, ReadOnlyDictionary<string, ClanInfo>>();
			AddRelation<IGetReconnectableGameRequest, GetReconnectableGameRequest>();
			AddRelation<IUnregisterPlayerFromReconnectableGameClientRequest, UnregisterPlayerFromReconnectableGameClientRequest>();
			AddRelation<ISetParametersForSinglePlayerGameRequest, SetParametersForSinglePlayerGameRequest, SetParametersForSinglePlayerGameDependency>();
			AddRelation<ISetParametersForCampaignRequest, SetParametersForCampaignRequest, SetParametersForCampaignDependency>();
		}
	}
}
