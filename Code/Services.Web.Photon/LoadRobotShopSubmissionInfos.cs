using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;

namespace Services.Web.Photon
{
	internal sealed class LoadRobotShopSubmissionInfos : WebServicesRequest<LoadRobotShopSubmissionInfosResult>, ILoadRobotShopSubmissionInfos, IServiceRequest, IAnswerOnComplete<LoadRobotShopSubmissionInfosResult>
	{
		protected override byte OperationCode => 95;

		public LoadRobotShopSubmissionInfos()
			: base("strGenericError", "strGenericErrorQuit", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override LoadRobotShopSubmissionInfosResult ProcessResponse(OperationResponse response)
		{
			Hashtable val = response.Parameters[100] as Hashtable;
			return new LoadRobotShopSubmissionInfosResult(Convert.ToUInt32(val.get_Item((object)"submissionCount")), Convert.ToUInt32(val.get_Item((object)"maxSubmissions")));
		}
	}
}
