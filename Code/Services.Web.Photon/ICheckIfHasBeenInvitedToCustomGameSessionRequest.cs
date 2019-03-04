using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface ICheckIfHasBeenInvitedToCustomGameSessionRequest : IServiceRequest, IAnswerOnComplete<CheckIfHasBeenInvitedToCustomGameSessionRequestResponse>
	{
		void ClearCache();
	}
}
