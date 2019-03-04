using Services;
using Services.Photon;

namespace ChatServiceLayer.Photon
{
	internal abstract class ChatRequest : PhotonRequest
	{
		protected override short UnexpectedErrorCode => 1;

		protected override string ServerName => ServicesServerNames.ChatServices;

		protected override byte GuidParameterCode => 0;

		public ChatRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
		}

		public override void Execute()
		{
			PhotonChatUtility.Instance.GetRequestConnectionAndExecuteRequest(new PhotonRequestContainer(this));
		}
	}
	internal abstract class ChatRequest<TData> : PhotonRequest<TData>
	{
		protected override short UnexpectedErrorCode => 1;

		protected override string ServerName => ServicesServerNames.ChatServices;

		protected override byte GuidParameterCode => 0;

		public ChatRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
		}

		public override void Execute()
		{
			PhotonChatUtility.Instance.GetRequestConnectionAndExecuteRequest(new PhotonRequestContainer(this));
		}
	}
}
