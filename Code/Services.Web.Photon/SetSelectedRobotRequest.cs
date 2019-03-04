using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class SetSelectedRobotRequest : WebServicesRequest<SetSelectedRobotDependency>, ISetSelectedRobotRequest, IServiceRequest<SetSelectedRobotDependency>, IAnswerOnComplete<SetSelectedRobotDependency>, IServiceRequest
	{
		private SetSelectedRobotDependency _dependency;

		protected override byte OperationCode => 44;

		public SetSelectedRobotRequest()
			: base("strRobocloudError", "strUnableSetCurrentRobot", 0)
		{
		}

		public void Inject(SetSelectedRobotDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[48] = Convert.ToInt32(_dependency.currentSlotId);
			val.OperationCode = OperationCode;
			return val;
		}

		protected override SetSelectedRobotDependency ProcessResponse(OperationResponse operationResponse)
		{
			CacheDTO.currentGarageSlot = new uint[1]
			{
				_dependency.currentSlotId
			};
			return _dependency;
		}
	}
}
