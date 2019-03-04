using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon.Tencent
{
	internal class IsPlayerRegisteredRequest_Tencent : WebServicesRequest<bool>, IIsPlayerRegisteredRequest_Tencent, IServiceRequest<IsPlayerRegisteredDependency>, IAnswerOnComplete<bool>, IServiceRequest
	{
		private IsPlayerRegisteredDependency _dependency;

		protected override byte OperationCode => 203;

		public IsPlayerRegisteredRequest_Tencent()
			: base("strRobocloudError", "strTencentIsPlayerRegisteredError", 0)
		{
		}

		public void Inject(IsPlayerRegisteredDependency dependency)
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
			val.OperationCode = OperationCode;
			return val;
		}

		protected override bool ProcessResponse(OperationResponse response)
		{
			return Convert.ToBoolean(response.Parameters[220]);
		}
	}
}
