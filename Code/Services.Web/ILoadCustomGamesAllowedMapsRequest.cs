using Svelto.ServiceLayer;

namespace Services.Web
{
	internal interface ILoadCustomGamesAllowedMapsRequest : IServiceRequest, IAnswerOnComplete<CustomGamesAllowedMapsData>
	{
	}
}
