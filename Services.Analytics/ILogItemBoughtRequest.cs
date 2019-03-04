using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogItemBoughtRequest : IServiceRequest<LogItemBoughtDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
