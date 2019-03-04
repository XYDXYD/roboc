using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogSettingsChangedRequest : IServiceRequest<LogSettingsChangedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
