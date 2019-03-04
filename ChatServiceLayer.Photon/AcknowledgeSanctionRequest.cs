using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace ChatServiceLayer.Photon
{
	internal class AcknowledgeSanctionRequest : ChatRequest, IAcknowledgeSanctionRequest, IServiceRequest<Sanction>, IServiceRequest
	{
		private Sanction _sanction;

		protected override byte OperationCode => 6;

		public AcknowledgeSanctionRequest()
			: base("strRobocloudError", "strErrorAcknowledgingSanction", 0)
		{
		}

		public void Inject(Sanction sanction)
		{
			_sanction = sanction;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(9, _sanction.SanctionType);
			dictionary.Add(8, _sanction.Issued.ToString("o"));
			Dictionary<byte, object> dictionary2 = val.Parameters = dictionary;
			return val;
		}
	}
}
