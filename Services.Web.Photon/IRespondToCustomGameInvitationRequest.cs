using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IRespondToCustomGameInvitationRequest : IServiceRequest<RespondToCustomGameInvitationDependancy>, IAnswerOnComplete<ReplyToCustomGameInviteResponseCode>, IServiceRequest
	{
		void ClearCache();
	}
}
