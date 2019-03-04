using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IKickFromCustomGameRequest : IServiceRequest<KickFromCustomGameRequestDependancy>, IAnswerOnComplete<KickFromCustomGameResponse>, IServiceRequest
	{
	}
}
