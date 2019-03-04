using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface IAddOrUpdateSanctionRequest : IServiceRequest<AddOrUpdateSanctionDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
