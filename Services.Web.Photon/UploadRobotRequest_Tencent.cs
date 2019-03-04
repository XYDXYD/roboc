using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class UploadRobotRequest_Tencent : WebServicesRequest<bool>, IUploadRobotRequest, IServiceRequest<UploadRobotDependency>, IAnswerOnComplete<bool>, IServiceRequest
	{
		private UploadRobotDependency _dependency;

		protected override byte OperationCode => 96;

		public UploadRobotRequest_Tencent()
			: base("strGenericError", "strGenericErrorQuit", 0)
		{
		}

		public void Inject(UploadRobotDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[101] = _dependency.ToDictionary();
			val.Parameters[99] = GetBuildNo();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override bool ProcessResponse(OperationResponse response)
		{
			return (bool)response.Parameters[103];
		}

		private string GetBuildNo()
		{
			CheckGameVersion.BuildVersionInfo buildVersionInfo = CheckGameVersion.GetBuildVersionInfo();
			return buildVersionInfo.VersionNumber.ToString();
		}
	}
}
