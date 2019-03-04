using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadPrebuiltRobotDataRequest : WebServicesCachedRequest<PrebuiltRobotsDependency>, ILoadPrebuiltRobotDataRequest, ITask, IServiceRequest, IAnswerOnComplete<PrebuiltRobotsDependency>, IAbstractTask
	{
		protected override byte OperationCode => 4;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadPrebuiltRobotDataRequest()
			: base("strRobocloudError", "strLoadPrebuiltRobotsError", 3)
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

		protected override PrebuiltRobotsDependency ProcessResponse(OperationResponse response)
		{
			PrebuiltRobotsDependency result = new PrebuiltRobotsDependency((Dictionary<string, Hashtable>)response.Parameters[1]);
			isDone = true;
			return result;
		}

		void ILoadPrebuiltRobotDataRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
