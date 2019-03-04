using Svelto.ServiceLayer;

namespace Services.Web
{
	internal interface IAdjustCustomGameConfigRequest : IServiceRequest<AdjustCustomGameConfigRequestDependancy>, IAnswerOnComplete<AdjustCustomGameConfigurationResponse>, IServiceRequest
	{
	}
}
