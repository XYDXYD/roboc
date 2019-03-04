using Svelto.ServiceLayer;

namespace Services.Web
{
	internal interface ICustomGameRobotTierChangedRequest : IServiceRequest<int>, IAnswerOnComplete, IServiceRequest
	{
		void ClearCache();
	}
}
