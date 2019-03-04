using Services.Web.Photon;
using SinglePlayerServiceLayer.Requests.Photon;
using Svelto.ServiceLayer;

namespace SinglePlayerServiceLayer
{
	internal class SinglePlayerRequestFactory : ServiceRequestFactory, ISinglePlayerRequestFactory, IServiceRequestFactory
	{
		public SinglePlayerRequestFactory()
		{
			AddRelation<ISinglePlayerSaveResultRequest, SinglePlayerSaveResultRequest, SaveGameAwardsRequestDependency>();
			AddRelation<ISinglePlayerLoadTdmAiRobotsRequest, SinglePlayerLoadTdmAiRobotsRequest>();
			AddRelation<IGetCampaignWavesDataRequest, GetCampaignWavesDataRequest, GetCampaignWavesDependency>();
			AddRelation<ILoadSinglePlayerCampaignsRequest, LoadSinglePlayerCampaignsRequest>();
			AddRelation<IValidateCampaignRobotRequest, ValidateCampaignRobotRequest, string>();
			AddRelation<IUpdatePlayerCompletedCampaignWaveRequest, UpdatePlayerCompletedCampaignWaveRequest, UpdatePlayerCompletedCampaignWaveDependency>();
			AddRelation<IGetLastCompletedCampaignRequest, GetLastCompletedCampaignRequest>();
			AddRelation<IMarkLastCompletedCampaignAsSeenRequest, MarkLastCompletedCampaignAsSeenRequest>();
			AddRelation<IGetGarageBayUniqueIdRequest, GetGarageBayUniqueIdRequest>();
			AddRelation<IGetRobotBayCustomisationsRequest, GetRobotBayCustomisationsRequest, string>();
		}
	}
}
