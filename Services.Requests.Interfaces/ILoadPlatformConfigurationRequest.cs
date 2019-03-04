using Svelto.ServiceLayer;

namespace Services.Requests.Interfaces
{
	internal interface ILoadPlatformConfigurationRequest : IServiceRequest, IAnswerOnComplete<PlatformConfigurationSettings>
	{
	}
}
