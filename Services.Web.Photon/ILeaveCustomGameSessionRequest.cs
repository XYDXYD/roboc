using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface ILeaveCustomGameSessionRequest : IServiceRequest, IAnswerOnComplete<SessionLeaveResponseCode>
	{
		void ClearCache();
	}
}
