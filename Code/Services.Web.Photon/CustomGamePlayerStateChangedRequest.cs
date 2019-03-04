using CustomGames;
using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class CustomGamePlayerStateChangedRequest : WebServicesRequest, ICustomGamePlayerStateChangedRequest, IServiceRequest<CustomGamePlayerSessionStatus>, IAnswerOnComplete, IServiceRequest
	{
		private CustomGamePlayerSessionStatus _desiredPlayerState;

		protected override byte OperationCode => 152;

		public CustomGamePlayerStateChangedRequest()
			: base("strRobocloudError", "strCustomGameCustomGamePlayerStateChangedRequestError", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>
			{
				{
					188,
					_desiredPlayerState
				}
			};
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}

		public void Inject(CustomGamePlayerSessionStatus desiredPlayerState)
		{
			_desiredPlayerState = desiredPlayerState;
		}
	}
}
