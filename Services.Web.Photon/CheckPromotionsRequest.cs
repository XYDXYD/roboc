using ExitGames.Client.Photon;
using RoboCraft.MiniJSON;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class CheckPromotionsRequest : WebServicesRequest<ClaimPendingPromotionsResponse>, ICheckPromotionsRequest, IServiceRequest<string>, IAnswerOnComplete<ClaimPendingPromotionsResponse>, IServiceRequest
	{
		private string _steamId;

		protected override byte OperationCode => 52;

		public CheckPromotionsRequest()
			: base("strRobocloudError", "strUnableToCheckPromotions", 0)
		{
		}

		public void Inject(string dependency)
		{
			_steamId = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[62] = _steamId;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override ClaimPendingPromotionsResponse ProcessResponse(OperationResponse operationResponse)
		{
			string[] promotions = (string[])operationResponse.Parameters[63];
			string text = (string)operationResponse.Parameters[121];
			string text2 = (string)operationResponse.Parameters[128];
			long cosmeticCreditsAwarded = (long)operationResponse.Parameters[86];
			bool roboPassAwarded = (bool)operationResponse.Parameters[4];
			Dictionary<string, object> steamPromotions = Json.Deserialize(text) as Dictionary<string, object>;
			Dictionary<string, object> cubesAwarded = Json.Deserialize(text2) as Dictionary<string, object>;
			return new ClaimPendingPromotionsResponse(promotions, steamPromotions, cubesAwarded, cosmeticCreditsAwarded, roboPassAwarded);
		}

		protected override void OnFailed(Exception exception)
		{
			base.OnFailed(exception);
		}
	}
}
