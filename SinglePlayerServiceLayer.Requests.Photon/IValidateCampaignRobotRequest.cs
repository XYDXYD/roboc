using Svelto.ServiceLayer;

namespace SinglePlayerServiceLayer.Requests.Photon
{
	internal interface IValidateCampaignRobotRequest : IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
	}
}
