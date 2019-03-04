using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class UploadCustomAvatarRequest : WebServicesRequest, IUploadCustomAvatarRequest, IServiceRequest<byte[]>, IAnswerOnComplete, IServiceRequest
	{
		private byte[] _customAvatar;

		protected override byte OperationCode => 112;

		public UploadCustomAvatarRequest()
			: base("strRobocloudError", "strErrorUploadingAvatar", 0)
		{
		}

		public void Inject(byte[] customAvatar)
		{
			_customAvatar = customAvatar;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(131, _customAvatar);
			dictionary.Add(132, CustomAvatarFormat.Jpg);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.Parameters = parameters;
			return val;
		}

		protected override void SuccessfulResponse(OperationResponse response)
		{
			base.SuccessfulResponse(response);
		}
	}
}
