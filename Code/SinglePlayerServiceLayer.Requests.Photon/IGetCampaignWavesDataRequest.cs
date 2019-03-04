using Svelto.ServiceLayer;

namespace SinglePlayerServiceLayer.Requests.Photon
{
	internal interface IGetCampaignWavesDataRequest : IServiceRequest<GetCampaignWavesDependency>, IAnswerOnComplete<CampaignWavesDifficultyData>, IServiceRequest
	{
		void ClearCache();
	}
}
