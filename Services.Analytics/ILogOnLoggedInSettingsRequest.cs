using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogOnLoggedInSettingsRequest : IServiceRequest<LogOnLoggedInSettingsDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
