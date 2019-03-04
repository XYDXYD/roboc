using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IGetRobotBayCustomisationsRequest : IServiceRequest<string>, IAnswerOnComplete<GetRobotBayCustomisationsResponse>, IServiceRequest
	{
		void ClearCache();
	}
}
