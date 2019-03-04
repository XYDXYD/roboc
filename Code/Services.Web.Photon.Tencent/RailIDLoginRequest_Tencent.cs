using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon.Tencent
{
	internal class RailIDLoginRequest_Tencent : WebServicesRequest<RailIDLoginResponse>, IRailIDLoginRequest_Tencent, IServiceRequest<RailIDLoginDependency>, IAnswerOnComplete<RailIDLoginResponse>, IServiceRequest
	{
		private RailIDLoginDependency _dependency;

		protected override byte OperationCode => 201;

		public RailIDLoginRequest_Tencent()
			: base("strRobocloudError", "strTencentUserNameLoginError", 0)
		{
		}

		public void Inject(RailIDLoginDependency dependency)
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
			val.OperationCode = OperationCode;
			return val;
		}

		protected override RailIDLoginResponse ProcessResponse(OperationResponse response)
		{
			string legacyName_ = Convert.ToString(response.Parameters[30]);
			string authToken_ = Convert.ToString(response.Parameters[218]);
			string displayName_ = Convert.ToString(response.Parameters[2]);
			return new RailIDLoginResponse(legacyName_, authToken_, displayName_);
		}
	}
}
