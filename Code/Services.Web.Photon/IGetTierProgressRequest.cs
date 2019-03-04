using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IGetTierProgressRequest : IServiceRequest, IAnswerOnComplete<TierProgress[]>
	{
	}
}
