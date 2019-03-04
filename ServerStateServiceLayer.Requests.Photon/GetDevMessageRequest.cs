using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System;
using System.Text;

namespace ServerStateServiceLayer.Requests.Photon
{
	internal class GetDevMessageRequest : WebServicesRequest<DevMessage>, IGetDevMessageRequest, IAnswerOnComplete<DevMessage>, IServiceRequest
	{
		protected override byte OperationCode => 8;

		public GetDevMessageRequest()
			: base("strRobocloudError", "strUnableLoadDevMsg", 3)
		{
		}

		protected override DevMessage ProcessResponse(OperationResponse response)
		{
			byte[] bytes = (byte[])response.Parameters[2];
			return new DevMessage(Encoding.UTF8.GetString(bytes), Convert.ToInt32(response.Parameters[15]));
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}
	}
}
