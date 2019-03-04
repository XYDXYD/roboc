using CustomGames;
using Svelto.ServiceLayer;

namespace Services.Web
{
	internal interface ICustomGamePlayerStateChangedRequest : IServiceRequest<CustomGamePlayerSessionStatus>, IAnswerOnComplete, IServiceRequest
	{
	}
}
