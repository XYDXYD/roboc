using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface ILoadPlayerRobotRankingRequest : IServiceRequest<string>, IAnswerOnComplete<RankingAndCPU>, IServiceRequest
	{
		void ClearCache();
	}
}
