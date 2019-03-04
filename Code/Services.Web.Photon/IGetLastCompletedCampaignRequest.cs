using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IGetLastCompletedCampaignRequest : IServiceRequest, IAnswerOnComplete<LastCompletedCampaign?>
	{
	}
}
