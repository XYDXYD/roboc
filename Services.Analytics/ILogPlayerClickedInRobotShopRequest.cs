using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerClickedInRobotShopRequest : IServiceRequest<int>, IAnswerOnComplete, IServiceRequest
	{
	}
}
