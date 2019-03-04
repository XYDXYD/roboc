using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IGetPlatoonPendingInviteRequest : IServiceRequest, IAnswerOnComplete<PlatoonInvite>
	{
		void ForceRefresh();
	}
}
