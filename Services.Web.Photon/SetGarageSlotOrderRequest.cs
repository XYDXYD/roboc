using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class SetGarageSlotOrderRequest : WebServicesRequest<SetGarageSlotOrderDependency>, ISetGarageSlotOrderRequest, IServiceRequest<SetGarageSlotOrderDependency>, IAnswerOnComplete<SetGarageSlotOrderDependency>, IServiceRequest
	{
		private SetGarageSlotOrderDependency _dependency;

		protected override byte OperationCode => 114;

		public SetGarageSlotOrderRequest()
			: base("strRobocloudError", "strUnableSetGarageSlotOrderRobot", 0)
		{
		}

		public void Inject(SetGarageSlotOrderDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			int[] value = _dependency.currentGarageSlotOrder.ToArray();
			val.Parameters[58] = value;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override SetGarageSlotOrderDependency ProcessResponse(OperationResponse operationResponse)
		{
			CacheDTO.garageSlotOrder.FastClear();
			CacheDTO.garageSlotOrder.AddRange(_dependency.currentGarageSlotOrder);
			return _dependency;
		}
	}
}
