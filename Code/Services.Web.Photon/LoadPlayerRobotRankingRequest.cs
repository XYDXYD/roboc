using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadPlayerRobotRankingRequest : WebServicesCachedRequest<RankingAndCPU>, ILoadPlayerRobotRankingRequest, IServiceRequest<string>, IAnswerOnComplete<RankingAndCPU>, IServiceRequest
	{
		private string _dependency;

		protected override byte OperationCode => 79;

		public LoadPlayerRobotRankingRequest()
			: base("strGenericError", "strLoadPlayerRobotRankingError", 3)
		{
		}

		public void Inject(string dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[30] = _dependency;
			return val;
		}

		protected override RankingAndCPU ProcessResponse(OperationResponse response)
		{
			int ranking = (int)response.Parameters[84];
			int totalCPU = (int)response.Parameters[177];
			int totalCosmeticCPU = (int)response.Parameters[176];
			return new RankingAndCPU(ranking, totalCPU, totalCosmeticCPU);
		}

		void ILoadPlayerRobotRankingRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
