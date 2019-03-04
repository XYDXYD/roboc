using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class SubmitCRFRatingRequest : WebServicesRequest, ISubmitCRFRatingRequest, IServiceRequest<SubmitCRFRatingDependency>, IAnswerOnComplete, IServiceRequest
	{
		private SubmitCRFRatingDependency _parameters;

		protected override byte OperationCode => 90;

		public SubmitCRFRatingRequest()
			: base("strRobotShopError", "strErrorPostCommunityShopRatings", 0)
		{
		}

		public void Inject(SubmitCRFRatingDependency parameters)
		{
			_parameters = new SubmitCRFRatingDependency();
			_parameters.slotId = parameters.slotId;
			_parameters.combatRating = parameters.combatRating;
			_parameters.cosmeticRating = parameters.cosmeticRating;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[43] = _parameters.slotId;
			val.Parameters[97] = _parameters.combatRating;
			val.Parameters[98] = _parameters.cosmeticRating;
			val.Parameters[99] = GetBuildNo();
			return val;
		}

		private string GetBuildNo()
		{
			CheckGameVersion.BuildVersionInfo buildVersionInfo = CheckGameVersion.GetBuildVersionInfo();
			return buildVersionInfo.VersionNumber.ToString();
		}
	}
}
