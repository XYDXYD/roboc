using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IInviteToPlatoonRequest : IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
	}
}
