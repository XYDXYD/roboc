using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon.Tencent
{
	internal class RegisterUserRequest_Tencent : WebServicesRequest<RegisterUserReturn>, IRegisterUserRequest_Tencent, IServiceRequest<RegisterUserDependency>, IAnswerOnComplete<RegisterUserReturn>, IServiceRequest
	{
		private RegisterUserDependency _dependency;

		protected override byte OperationCode => 202;

		public RegisterUserRequest_Tencent()
			: base("strRobocloudError", "strTencentRegisterUserError", 0)
		{
		}

		public void Inject(RegisterUserDependency dependency)
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
			val.Parameters[219] = _dependency.sessionID;
			val.Parameters[2] = _dependency.displayName;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override RegisterUserReturn ProcessResponse(OperationResponse response)
		{
			string token_ = Convert.ToString(response.Parameters[218]);
			string displayName_ = Convert.ToString(response.Parameters[2]);
			string userName_ = Convert.ToString(response.Parameters[30]);
			return new RegisterUserReturn(token_, displayName_, userName_);
		}
	}
}
