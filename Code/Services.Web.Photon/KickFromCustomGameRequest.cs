using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class KickFromCustomGameRequest : WebServicesRequest<KickFromCustomGameResponse>, IKickFromCustomGameRequest, IServiceRequest<KickFromCustomGameRequestDependancy>, IAnswerOnComplete<KickFromCustomGameResponse>, IServiceRequest
	{
		private KickFromCustomGameRequestDependancy _dependancy;

		protected override byte OperationCode => 150;

		public KickFromCustomGameRequest()
			: base("strCustomGameError", "strCustomGameKickFromCustomGameError", 0)
		{
		}

		public void Inject(KickFromCustomGameRequestDependancy dependency)
		{
			_dependancy = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[183] = _dependancy.UserToKick;
			val.Parameters = dictionary;
			return val;
		}

		protected override KickFromCustomGameResponse ProcessResponse(OperationResponse response)
		{
			return (KickFromCustomGameResponse)response.get_Item((byte)168);
		}
	}
}
