using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IKickFromPlatoonRequest : IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
	}
}
