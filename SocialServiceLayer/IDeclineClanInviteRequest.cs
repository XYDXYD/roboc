using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IDeclineClanInviteRequest : IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
	}
}
