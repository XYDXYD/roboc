using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal class CheckGameVersionRequest : WebServicesRequest<CheckGameVersionData>, ICheckGameVersionRequest, IServiceRequest, IAnswerOnComplete<CheckGameVersionData>
	{
		protected override byte OperationCode => 103;

		public CheckGameVersionRequest()
			: base("strRobocloudError", "strLoadGameVersionError", 0)
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

		protected override CheckGameVersionData ProcessResponse(OperationResponse response)
		{
			int versionNumber = (int)response.Parameters[112];
			return new CheckGameVersionData(versionNumber, string.Empty, 0);
		}
	}
}
