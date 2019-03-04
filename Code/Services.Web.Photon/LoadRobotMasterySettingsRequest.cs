using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadRobotMasterySettingsRequest : WebServicesCachedRequest<RobotMasterySettingsDependency>, ILoadRobotMasterySettingsRequest, ITask, IServiceRequest, IAnswerOnComplete<RobotMasterySettingsDependency>, IAbstractTask
	{
		protected override byte OperationCode => 73;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadRobotMasterySettingsRequest()
			: base("strRobocloudError", "strLoadMasterySettingsError", 3)
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

		protected override RobotMasterySettingsDependency ProcessResponse(OperationResponse response)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			RobotMasterySettingsDependency result = new RobotMasterySettingsDependency(response.Parameters[193]);
			isDone = true;
			return result;
		}

		void ILoadRobotMasterySettingsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
