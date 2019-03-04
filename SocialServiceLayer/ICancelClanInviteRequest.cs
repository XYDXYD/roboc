using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface ICancelClanInviteRequest : IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
	}
}
