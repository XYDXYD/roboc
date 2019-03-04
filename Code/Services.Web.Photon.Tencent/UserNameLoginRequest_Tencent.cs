using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Services.Web.Photon.Tencent
{
	internal class UserNameLoginRequest_Tencent : WebServicesRequest<UserNameLoginResult>, IUserNameLoginRequest_Tencent, IServiceRequest<UserNameLoginDependency>, IAnswerOnComplete<UserNameLoginResult>, IServiceRequest
	{
		private RSACryptoServiceProvider _RSA = new RSACryptoServiceProvider();

		private UserNameLoginDependency _dependency;

		protected override byte OperationCode => 200;

		public UserNameLoginRequest_Tencent()
			: base("strRobocloudError", "strTencentUserNameLoginError", 0)
		{
			_RSA.FromXmlString("<RSAKeyValue><Modulus>iYYsCmEnL2h6VtRYNKQY1hm9aGeux3ljOGRBB+Aj4ZXldAXjkmP5nP1wwHNZdWeC1Dyxql\r\nER44p4Gx37ypPV6xtnMvD+YMWnBhbHWGrGq18AGMblZRAweKQlsu7BEACWPiu8OtmjRa9CIajwUYLUPYTH/sLzzEk40jp5fwDTLik=\r\n</Modulus><Exponent>EQ==</Exponent></RSAKeyValue>");
		}

		public void Inject(UserNameLoginDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[30] = _dependency.username;
			val.Parameters[224] = Convert.ToBase64String(_RSA.Encrypt(Encoding.UTF8.GetBytes(_dependency.password), fOAEP: false));
			val.OperationCode = OperationCode;
			return val;
		}

		protected override UserNameLoginResult ProcessResponse(OperationResponse response)
		{
			string token_ = Convert.ToString(response.Parameters[218]);
			string displayname_ = Convert.ToString(response.Parameters[2]);
			return new UserNameLoginResult(token_, displayname_);
		}
	}
}
