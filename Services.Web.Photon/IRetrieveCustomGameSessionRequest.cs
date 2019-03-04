using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IRetrieveCustomGameSessionRequest : IServiceRequest, IAnswerOnComplete<RetrieveCustomGameSessionRequestData>
	{
		void ClearCache();
	}
}
