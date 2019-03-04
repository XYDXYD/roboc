using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface ISetPlatoonMemberStatusRequest : IServiceRequest<SetPlatoonMemberStatusDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
