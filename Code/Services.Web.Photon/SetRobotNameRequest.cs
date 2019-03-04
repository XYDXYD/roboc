using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class SetRobotNameRequest : WebServicesRequest<string>, ISetRobotNameRequest, IServiceRequest<SetRobotNameDependency>, IAnswerOnComplete<string>, IServiceRequest
	{
		private SetRobotNameDependency _dependency;

		protected override byte OperationCode => 46;

		public SetRobotNameRequest()
			: base("strRobocloudError", "strUnableSetCurrentRobot", 0)
		{
		}

		public void Inject(SetRobotNameDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[45] = Convert.ToInt32(_dependency.index);
			val.Parameters[42] = _dependency.name;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override string ProcessResponse(OperationResponse response)
		{
			CacheDTO.garageSlots[_dependency.index].name = _dependency.name;
			return _dependency.name;
		}
	}
}
