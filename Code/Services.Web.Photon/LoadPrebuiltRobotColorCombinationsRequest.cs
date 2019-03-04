using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadPrebuiltRobotColorCombinationsRequest : WebServicesCachedRequest<PrebuiltRobotColorCombinations>, ILoadPrebuiltRobotColorCombinationsRequest, ITask, IServiceRequest, IAnswerOnComplete<PrebuiltRobotColorCombinations>, IAbstractTask
	{
		protected override byte OperationCode => 37;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadPrebuiltRobotColorCombinationsRequest()
			: base("strRobocloudError", "strLoadPrebuiltRobotColorsError", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override PrebuiltRobotColorCombinations ProcessResponse(OperationResponse response)
		{
			string responseString = (string)response.Parameters[1];
			PrebuiltRobotColorCombinations result = new PrebuiltRobotColorCombinations(responseString);
			isDone = true;
			return result;
		}

		void ILoadPrebuiltRobotColorCombinationsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
