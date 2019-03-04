using ChatServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;

internal class PrivateChat
{
	[Inject]
	internal IChatRequestFactory chatRequestFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ChatPresenter chatPresenter
	{
		private get;
		set;
	}

	public string LastReceivedWhisperFrom
	{
		get;
		set;
	}

	public string LastReceivedWhisperFromDisplayName
	{
		get;
		set;
	}

	public void SendMessage(OutgoingPrivateChatMessage message)
	{
		ISendPrivateMessageRequest sendPrivateMessageRequest = chatRequestFactory.Create<ISendPrivateMessageRequest, PrivateMessageDependency>(new PrivateMessageDependency
		{
			Receiver = message.Receiver,
			Text = message.RawText
		});
		sendPrivateMessageRequest.SetAnswer(new ServiceAnswer(delegate
		{
			chatPresenter.LogLocalMessage(message);
		}, OnError)).Execute();
	}

	private void OnError(ServiceBehaviour serviceBehaviour)
	{
		string @string = StringTableBase<StringTable>.Instance.GetString(Enum.GetName(typeof(ChatReasonCode), serviceBehaviour.errorCode));
		chatPresenter.SystemMessage(@string);
	}
}
