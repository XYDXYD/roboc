using SinglePlayerCampaign.GUI.Mothership.DataTypes;
using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface ILoadSinglePlayerCampaignsRequest : IServiceRequest, IAnswerOnComplete<GetCampaignsRequestResult>
	{
	}
}
