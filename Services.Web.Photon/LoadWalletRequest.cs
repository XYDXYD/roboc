using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadWalletRequest : WebServicesCachedRequest<Wallet>, ILoadWalletRequest, IServiceRequest, IAnswerOnComplete<Wallet>
	{
		protected override byte OperationCode => 66;

		public LoadWalletRequest()
			: base("strRobocloudError", "strUnableLoadCurrencyBalance", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override Wallet ProcessResponse(OperationResponse response)
		{
			long robitsBalance = Convert.ToInt64(response.Parameters[74]);
			long cosmeticCreditsBalance = Convert.ToInt64(response.Parameters[87]);
			return new Wallet(robitsBalance, cosmeticCreditsBalance);
		}

		void ILoadWalletRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
