using Authentication;
using ChatServiceLayer;
using ExtensionMethods;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Runtime.CompilerServices;

internal class StandardChatCommands : IInitialize, IWaitForFrameworkDestruction
{
	[CompilerGenerated]
	private static Action<ServiceBehaviour> _003C_003Ef__mg_0024cache0;

	[Inject]
	internal ChatCommands ChatCommands
	{
		private get;
		set;
	}

	[Inject]
	internal ChatPresenter ChatPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal ChatChannelContainer ChatChannelContainer
	{
		private get;
		set;
	}

	[Inject]
	internal PrivateChat PrivateChat
	{
		private get;
		set;
	}

	[Inject]
	internal IChatRequestFactory ChatRequestFactory
	{
		private get;
		set;
	}

	public void OnDependenciesInjected()
	{
		ChatCommands.RegisterCommand("w", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strWhisperCommandUsage"), ProcessWhisper, 1);
		ChatCommands.RegisterCommand("msg", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strWhisperCommandAltUsage"), ProcessWhisper, 1);
		ChatCommands.RegisterCommand("group", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strGroupCommandUsage"), PlatoonMessageCommand, 1);
		ChatCommands.RegisterCommand("r", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strReplyCommandUsage"), ReplyCommand, 1);
	}

	public void OnFrameworkDestroyed()
	{
		ChatCommands.DeregisterCommand("w");
		ChatCommands.DeregisterCommand("msg");
		ChatCommands.DeregisterCommand("group");
		ChatCommands.DeregisterCommand("r");
	}

	private bool ProcessWhisper(string input)
	{
		ChatCommands.GetParam(input, out string param, out string message);
		if (param.Equals(User.Username, StringComparison.CurrentCultureIgnoreCase))
		{
			ChatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strErrCannotMessageSelf"));
			return true;
		}
		if (string.IsNullOrEmpty(param))
		{
			return false;
		}
		IGetCanSendPrivateMessageRequest getCanSendPrivateMessageRequest = ChatRequestFactory.Create<IGetCanSendPrivateMessageRequest>();
		getCanSendPrivateMessageRequest.Inject(param);
		getCanSendPrivateMessageRequest.SetAnswer(new ServiceAnswer<CanSendWhisperRequestResult>(delegate(CanSendWhisperRequestResult canSendResult)
		{
			OnGotCanSendWhisperResponse(canSendResult, message);
		}, ErrorWindow.ShowServiceErrorWindow));
		getCanSendPrivateMessageRequest.Execute();
		return true;
	}

	private void OnGotCanSendWhisperResponse(CanSendWhisperRequestResult canSendWhisperRequestResult, string message)
	{
		if (canSendWhisperRequestResult.canSendPrivateMessageResult == CanSandPrivateMessageResult.UserNotExists)
		{
			ChatPresenter.SystemMessage(Localization.Get("strUserNotExist", true));
		}
		else if (canSendWhisperRequestResult.canSendPrivateMessageResult == CanSandPrivateMessageResult.UserNotOnline)
		{
			ChatPresenter.SystemMessage(Localization.Get("strTheyNotOnline", true));
		}
		else if (string.IsNullOrEmpty(message))
		{
			ChatPresenter.CreateOrSelectWhisperChannel(canSendWhisperRequestResult.receiverName, canSendWhisperRequestResult.displayName);
		}
		else
		{
			ChatPresenter.SendWhisper(canSendWhisperRequestResult.receiverName, canSendWhisperRequestResult.displayName, message);
		}
	}

	private bool PlatoonMessageCommand(string input)
	{
		if (input.IsNullOrWhiteSpace())
		{
			return false;
		}
		if (ChatPresenter.PlatoonId == null)
		{
			ChatPresenter.SystemMessage(Localization.Get("strErrorNotInPlatoon", true));
			return true;
		}
		if (!ChatPresenter.PlatoonChannelExists())
		{
			throw new Exception("Unable to get platoon chat channel");
		}
		ChatPresenter.SendOutgoingMessage(input, null, ChatChannelContainer.GetPlatoonChannel());
		return true;
	}

	private bool ReplyCommand(string input)
	{
		if (input.IsNullOrWhiteSpace())
		{
			return false;
		}
		string lastReceivedWhisperFrom = PrivateChat.LastReceivedWhisperFrom;
		string lastReceivedWhisperFromDisplayName = PrivateChat.LastReceivedWhisperFromDisplayName;
		if (lastReceivedWhisperFrom.IsNullOrWhiteSpace() || lastReceivedWhisperFromDisplayName.IsNullOrWhiteSpace())
		{
			ChatPresenter.SystemMessage(Localization.Get("strNoWhispersReceived", true));
			return true;
		}
		ChatPresenter.SendWhisper(lastReceivedWhisperFrom, lastReceivedWhisperFromDisplayName, input);
		return true;
	}
}
