using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IRenameClanRequest : IServiceRequest<ClanRenameDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
