using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon.Tencent
{
	internal class CreateOrderRequest_Tencent : WebServicesRequest, ICreateOrderRequest_Tencent, IServiceRequest<CreateOrderDependency>, IAnswerOnComplete, IServiceRequest
	{
		private CreateOrderDependency _dependency;

		protected override byte OperationCode => 204;

		public CreateOrderRequest_Tencent()
			: base("strRobocloudError", "strTencentCreateOrderError", 0)
		{
		}

		public void Inject(CreateOrderDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[217] = _dependency.railID;
			val.Parameters[219] = _dependency.railSessionID;
			val.Parameters[221] = _dependency.sku;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
