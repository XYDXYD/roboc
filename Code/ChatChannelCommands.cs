using ChatServiceLayer;
using ExtensionMethods;
using Mothership;
using Services.Requests.Interfaces;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

internal class ChatChannelCommands : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
{
	public const int MaxChannelNameLength = 16;

	private const string PasswordPattern = "^[a-zA-Z0-9]+$";

	private readonly string[] _reservedChannelNames = new string[4]
	{
		"ALL",
		"TEAM",
		"CLAN",
		"PARTY"
	};

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
	internal IChatRequestFactory ChatRequestFactory
	{
		private get;
		set;
	}

	[Inject]
	internal IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	[Inject]
	internal LoadingIconPresenter loadingPresenter
	{
		private get;
		set;
	}

	public void OnFrameworkInitialized()
	{
		TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadPlatformConfigurationValues);
	}

	public void OnFrameworkDestroyed()
	{
		ChatCommands.DeregisterCommand("create");
		ChatCommands.DeregisterCommand("join");
		ChatCommands.DeregisterCommand("leave");
		ChatCommands.DeregisterCommand("list");
		ChatCommands.DeregisterCommand("clan");
	}

	private IEnumerator LoadPlatformConfigurationValues()
	{
		loadingPresenter.NotifyLoading("LoadingPlatformConfiguration");
		ILoadPlatformConfigurationRequest request = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
		TaskService<PlatformConfigurationSettings> task = request.AsTask();
		yield return new HandleTaskServiceWithError(task, delegate
		{
			loadingPresenter.NotifyLoading("LoadingPlatformConfiguration");
		}, delegate
		{
			loadingPresenter.NotifyLoadingDone("LoadingPlatformConfiguration");
		}).GetEnumerator();
		loadingPresenter.NotifyLoadingDone("LoadingPlatformConfiguration");
		if (task.succeeded)
		{
			if (task.result.CanCreateChatRooms)
			{
				ChatCommands.RegisterCommand("create", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strCreateCommandUsage"), CreateChannelCommand, 1, 2);
			}
			ChatCommands.RegisterCommand("join", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strJoinCommandUsage"), JoinChannelCommand, 1, 2);
			ChatCommands.RegisterCommand("leave", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strLeaveCommandUsage"), LeaveChannelCommand, 1);
			ChatCommands.RegisterCommand("list", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strListCommandUsage"), ListChannelsCommand, 0, 0);
			ChatCommands.RegisterCommand("clan", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strClanCommmandUsage"), ClanMessageCommand);
		}
		else
		{
			ErrorWindow.ShowServiceErrorWindow(task.behaviour);
		}
	}

	private bool CreateChannelCommand(string input)
	{
		ChatCommands.GetParam(input, out string param, out string remainder);
		string[] reservedChannelNames = _reservedChannelNames;
		foreach (string value in reservedChannelNames)
		{
			if (param.Equals(value, StringComparison.InvariantCultureIgnoreCase))
			{
				ChatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strChannelNameReserved"));
				return true;
			}
		}
		if (ChatPresenter.CheckForProfanity(param))
		{
			ChatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strChannelNameProfanity"));
			return true;
		}
		if (!SocialInputValidation.ValidateUserName(ref param))
		{
			ChatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strChannelNameIllegalChars"));
			return true;
		}
		if (param.Length > 16)
		{
			ChatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strChannelNameTooLong"));
			return true;
		}
		if (remainder.IsNullOrWhiteSpace())
		{
			remainder = null;
		}
		else if (!Regex.IsMatch(remainder, "^[a-zA-Z0-9]+$"))
		{
			ChatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strBadlyFormedPassword"));
			return true;
		}
		ChatChannelContainer.CreateNewChannel(param, remainder, OnNewChatChannelCreated);
		return true;
	}

	private bool JoinChannelCommand(string input)
	{
		ChatCommands.GetParam(input, out string channelName, out string password);
		if (password.IsNullOrWhiteSpace())
		{
			password = null;
		}
		ChatRequestFactory.Create<IGetAllPublicChannelNamesRequest>().SetAnswer(new ServiceAnswer<string[]>(delegate(string[] publicChannels)
		{
			JoinChannelUncertainType(publicChannels, channelName, password);
		}, OnFailedToGetPublicChannelNames)).Execute();
		return true;
	}

	private void JoinChannelUncertainType(string[] publicChannels, string channelName, string password)
	{
		foreach (string text in publicChannels)
		{
			if (text.Equals(channelName, StringComparison.OrdinalIgnoreCase))
			{
				ChatPresenter.JoinChannel(channelName, ChatChannelType.Public);
				return;
			}
		}
		ChatPresenter.JoinChannel(channelName, ChatChannelType.Custom, password);
	}

	private bool LeaveChannelCommand(string channelName)
	{
		IChatChannel channelUnknownType = ChatChannelContainer.GetChannelUnknownType(channelName);
		if (channelUnknownType == null)
		{
			ChatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strChannelNotJoined"));
			return true;
		}
		if (channelUnknownType.ChatChannelType != ChatChannelType.Custom && channelUnknownType.ChatChannelType != ChatChannelType.Public)
		{
			ChatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strChannelCannotBeLeft"));
			return true;
		}
		ChatPresenter.LeaveChannel(channelName, channelUnknownType.ChatChannelType);
		return true;
	}

	private bool ListChannelsCommand(string input)
	{
		ChatRequestFactory.Create<IGetAllPublicChannelNamesRequest>().SetAnswer(new ServiceAnswer<string[]>(OnGotPublicChannelNames, OnFailedToGetPublicChannelNames)).Execute();
		return true;
	}

	private void OnFailedToGetPublicChannelNames(ServiceBehaviour serviceBehaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
	}

	private void OnGotPublicChannelNames(string[] channelNames)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strAvailableChannelsTitle"));
		stringBuilder.Append("\n");
		stringBuilder.Append(string.Join(", ", channelNames));
		ChatPresenter.SystemMessage(stringBuilder.ToString());
	}

	private void OnNewChatChannelCreated(ChatChannel channel)
	{
		ChatPresenter.SetActiveChannel(channel);
		ChatPresenter.EventMessage(StringTableBase<StringTable>.Instance.GetString("strCreated"), channel.VisibleName, channel.ChatChannelType);
	}

	private bool ClanMessageCommand(string input)
	{
		if (ChatPresenter.ClanName == null)
		{
			ChatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strErrorNotInClan"));
			return true;
		}
		if (!ChatPresenter.ClanChannelExists())
		{
			throw new Exception("Unable to get clan chat channel");
		}
		if (input.IsNullOrWhiteSpace())
		{
			ChatPresenter.SelectClanChannel();
			return true;
		}
		ChatPresenter.SendOutgoingMessage(input, null, ChatChannelContainer.GetClanChannel());
		return true;
	}
}
