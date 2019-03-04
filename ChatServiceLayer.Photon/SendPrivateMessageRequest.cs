using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace ChatServiceLayer.Photon
{
	internal class SendPrivateMessageRequest : ChatRequest, ISendPrivateMessageRequest, IServiceRequest<PrivateMessageDependency>, IAnswerOnComplete, IServiceRequest
	{
		private string _receiver = string.Empty;

		private string _text = string.Empty;

		private string _chatLocation = string.Empty;

		protected override byte OperationCode => 3;

		public SendPrivateMessageRequest()
			: base("strRobocloudError", "strErrorSendingPrivateMessage", 0)
		{
		}

		public void Inject(PrivateMessageDependency dependency)
		{
			_receiver = dependency.Receiver;
			_text = dependency.Text;
			_chatLocation = dependency.ChatLocation;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(7, _receiver);
			dictionary.Add(2, _text);
			dictionary.Add(29, _chatLocation);
			Dictionary<byte, object> dictionary2 = val.Parameters = dictionary;
			return val;
		}
	}
}
