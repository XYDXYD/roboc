using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class SetRobotCustomisationsRequest : WebServicesRequest<SetRobotCustomisationDependency>, ISetRobotCustomisationsRequest, IServiceRequest<SetRobotCustomisationDependency>, IAnswerOnComplete<SetRobotCustomisationDependency>, ITask, IServiceRequest, IAbstractTask
	{
		private SetRobotCustomisationDependency _dependency;

		protected override byte OperationCode => 217;

		public bool isDone
		{
			get;
			private set;
		}

		public SetRobotCustomisationsRequest()
			: base("strRobocloudError", "strSetRobotCustomisationsRequestError", 0)
		{
		}

		public void Inject(SetRobotCustomisationDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[54] = Convert.ToString(_dependency.RobotUniqueID);
			val.Parameters[234] = Convert.ToString(_dependency.BaySkinID);
			val.Parameters[235] = Convert.ToString(_dependency.SpawnEffectID);
			val.Parameters[236] = Convert.ToString(_dependency.DeathEffectID);
			val.OperationCode = OperationCode;
			return val;
		}

		protected override SetRobotCustomisationDependency ProcessResponse(OperationResponse response)
		{
			isDone = true;
			CacheDTO.garageSlots[_dependency.SlotID].baySkinID = _dependency.BaySkinID;
			return _dependency;
		}
	}
}
