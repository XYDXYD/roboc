using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface ICheckIfCanJoinCustomGameQueueResponse : IServiceRequest, IAnswerOnComplete<CheckIfCanJoinCustomGameQueueResponse>
	{
		void ClearCache();
	}
}
