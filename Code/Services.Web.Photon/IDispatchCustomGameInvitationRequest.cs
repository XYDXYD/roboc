using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IDispatchCustomGameInvitationRequest : IServiceRequest<DispatchCustomGameInviteDependancy>, IAnswerOnComplete<DispatchCustomGameInviteResponse>, IServiceRequest
	{
	}
}
