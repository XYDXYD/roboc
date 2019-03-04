using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class ReportCommunityShopItemRequest : WebServicesRequest, IReportCommunityShopItemRequest, IServiceRequest<ReportCommunityShopItemDependency>, IAnswerOnComplete, IServiceRequest
	{
		private ReportCommunityShopItemDependency _parameters;

		protected override byte OperationCode => 94;

		public ReportCommunityShopItemRequest()
			: base("strGenericError", "strGenericErrorQuit", 0)
		{
		}

		public void Inject(ReportCommunityShopItemDependency parameters)
		{
			_parameters = new ReportCommunityShopItemDependency();
			_parameters.itemId = parameters.itemId;
			_parameters.reason = parameters.reason;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[94] = _parameters.itemId;
			val.Parameters[9] = _parameters.reason;
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
