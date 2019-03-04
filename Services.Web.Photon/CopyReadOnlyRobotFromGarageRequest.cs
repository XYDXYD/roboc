using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class CopyReadOnlyRobotFromGarageRequest : WebServicesRequest, ICopyReadOnlyRobotFromGarageRequest, IServiceRequest<int>, IAnswerOnComplete, IServiceRequest
	{
		private int _garageID = -1;

		protected override byte OperationCode => 69;

		public CopyReadOnlyRobotFromGarageRequest()
			: base("strRobocloudError", "strUnableToCopyRobotFromGarage", 0)
		{
		}

		public void Inject(int garageID)
		{
			_garageID = garageID;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[43] = _garageID;
			val.Parameters[213] = StringTableBase<StringTable>.Instance.GetString("strCopyRobotExtension");
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
