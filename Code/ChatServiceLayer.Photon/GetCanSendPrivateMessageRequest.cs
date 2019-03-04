using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace ChatServiceLayer.Photon
{
	internal class GetCanSendPrivateMessageRequest : ChatRequest<CanSendWhisperRequestResult>, IGetCanSendPrivateMessageRequest, IServiceRequest<string>, IAnswerOnComplete<CanSendWhisperRequestResult>, IServiceRequest
	{
		private string _userName;

		protected override byte OperationCode => 14;

		public GetCanSendPrivateMessageRequest()
			: base("strGetUserExistsErrorTitle", "strGetUserExistsErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			Dictionary<byte, object> parameters = new Dictionary<byte, object>();
			parameters.AddParameter(ChatParameterCode.UserName, _userName);
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = parameters;
			return val;
		}

		protected override CanSendWhisperRequestResult ProcessResponse(OperationResponse response)
		{
			CanSandPrivateMessageResult parameter = response.Parameters.GetParameter<CanSandPrivateMessageResult>(ChatParameterCode.CanSendPrivateMessage);
			string receiverName = string.Empty;
			string displayName = string.Empty;
			if (parameter == CanSandPrivateMessageResult.Ok)
			{
				receiverName = response.Parameters.GetParameter<string>(ChatParameterCode.UserName);
				displayName = response.Parameters.GetParameter<string>(ChatParameterCode.DisplayName);
			}
			return new CanSendWhisperRequestResult(receiverName, displayName, parameter);
		}

		public void Inject(string userName)
		{
			_userName = userName;
		}
	}
}
