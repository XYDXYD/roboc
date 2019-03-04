using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface ICreateCustomGameSessionRequest : IServiceRequest, IAnswerOnComplete<SessionCreationResponseCode>
	{
		void ClearCache();
	}
}
