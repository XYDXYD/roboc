using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IGetTierProgressNotificationRequest : IServiceRequest, IAnswerOnComplete<TierProgressNotificationData>
	{
	}
}
