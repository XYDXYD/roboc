using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IChangeClanDataRequest : IServiceRequest<ChangeClanDataDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
