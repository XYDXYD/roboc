using Authentication;
using ChatServiceLayer;
using ExtensionMethods;
using Mothership;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

internal abstract class AccountSanctions : IInitialize, IWaitForFrameworkDestruction
{
	protected bool shouldShowSanctionDialog;

	private IServiceEventContainer _chatEventContainer;

	private IServiceEventContainer _socialEventContainer;

	private GenericErrorData _genericErrorData;

	[CompilerGenerated]
	private static Action _003C_003Ef__mg_0024cache0;

	[Inject]
	internal ChatCommands chatCommands
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

	[Inject]
	internal IChatRequestFactory chatRequestFactory
	{
		private get;
		set;
	}

	[Inject]
	internal IChatEventContainerFactory ChatEventContainerFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ISocialRequestFactory socialRequestFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ISocialEventContainerFactory SocialEventContainerFactory
	{
		private get;
		set;
	}

	[Inject]
	internal GenericInfoDisplay GenericInfoDisplay
	{
		private get;
		set;
	}

	[Inject]
	internal ICommandFactory CommandFactory
	{
		get;
		set;
	}

	[Inject]
	internal IContextNotifer contextNotifier
	{
		private get;
		set;
	}

	[Inject]
	internal LoadingIconPresenter loadingIconPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal IGUIInputController guiInputController
	{
		private get;
		set;
	}

	[Inject]
	internal DisableChatInputObservable disableChatObservable
	{
		private get;
		set;
	}

	void IInitialize.OnDependenciesInjected()
	{
		chatCommands.RegisterCommand("silence", ChatCommands.CommandData.AccessLevel.ModAdminOrDev, "/silence <username> <reason(optional, must be supplied if a duration is supplied)> <duration_days(optional, defaults to 1(day))>", SilenceCommand);
		chatCommands.RegisterCommand("unsilence", ChatCommands.CommandData.AccessLevel.ModAdminOrDev, "/unsilence <username>", UnsilenceCommand);
		chatCommands.RegisterCommand("suspend", ChatCommands.CommandData.AccessLevel.AdminOrDev, "/suspend <username> <reason> <duration_days(optional, defaults to permanent)>", SuspendCommand);
		chatCommands.RegisterCommand("unsuspend", ChatCommands.CommandData.AccessLevel.AdminOrDev, "/suspend <username>", UnsuspendCommand);
		chatCommands.RegisterCommand("warn", ChatCommands.CommandData.AccessLevel.ModAdminOrDev, "/warn <username> <reason>", WarnCommand);
		chatCommands.RegisterCommand("note", ChatCommands.CommandData.AccessLevel.ModAdminOrDev, "/note <username> <note>", NoteCommand);
		chatCommands.RegisterCommand("renameclan", ChatCommands.CommandData.AccessLevel.AdminOrDev, "/renameclan <oldName> <newName>", RenameOffensiveClanCommand, 2, 2);
		_chatEventContainer = ChatEventContainerFactory.Create();
		_chatEventContainer.ListenTo<ISanctionEventListener, Sanction>(SanctionReceived);
		_socialEventContainer = SocialEventContainerFactory.Create();
		_socialEventContainer.ListenTo<IClanRenamedEventListener, ClanRenameDependency>(ClanRenamed);
	}

	public abstract void HandleSuspensionEvent();

	public abstract bool CanShowSanctionDialog();

	public IEnumerator RefreshData()
	{
		ICheckPendingSanctionRequest checkPendingRequest = chatRequestFactory.Create<ICheckPendingSanctionRequest>();
		TaskService<bool> checkPendingRequestTaskService = new TaskService<bool>(checkPendingRequest);
		yield return new HandleTaskServiceWithError(checkPendingRequestTaskService, delegate
		{
			loadingIconPresenter.NotifyLoading("RefreshAccountSanctionsData", opaque: true);
		}, delegate
		{
			loadingIconPresenter.NotifyLoadingDone("RefreshAccountSanctionsData");
		}).GetEnumerator();
		if (checkPendingRequestTaskService.succeeded && checkPendingRequestTaskService.result)
		{
			while (_genericErrorData == null)
			{
				yield return null;
			}
			GenericInfoDisplay.ShowInfoDialogue(_genericErrorData);
			while (!CanShowSanctionDialog())
			{
				yield return null;
			}
		}
	}

	public void MothershipFlowCompleted()
	{
		shouldShowSanctionDialog = true;
		if (_genericErrorData != null)
		{
			ErrorWindow.ShowErrorWindow(_genericErrorData);
		}
	}

	void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		if (_chatEventContainer != null)
		{
			_chatEventContainer.Dispose();
			_chatEventContainer = null;
		}
		if (_socialEventContainer != null)
		{
			_socialEventContainer.Dispose();
			_socialEventContainer = null;
		}
	}

	private bool SilenceCommand(string contents)
	{
		if (contents.IsNullOrWhiteSpace())
		{
			return false;
		}
		ChatCommands.GetParam(contents, out string param, out string remainder);
		if (param.IsNullOrWhiteSpace())
		{
			return false;
		}
		if (IsMe(param))
		{
			chatPresenter.SystemMessage("You cannot do that to yourself");
			return true;
		}
		string text = null;
		int result = 1;
		if (!remainder.IsNullOrWhiteSpace())
		{
			int num = remainder.LastIndexOf(" ", StringComparison.Ordinal);
			string s = remainder.Substring(num + 1);
			if (num != -1 && int.TryParse(s, out result))
			{
				if (result < 1)
				{
					chatPresenter.SystemMessage("Cannot silence for less than 1 day");
					return true;
				}
				text = remainder.Substring(0, num);
			}
			else
			{
				result = 1;
				text = remainder;
			}
			if (result > 1 && text.IsNullOrWhiteSpace())
			{
				return false;
			}
			if (int.TryParse(text, out int _))
			{
				return false;
			}
		}
		string str = $"Silencing {param} for {result} days(s)";
		str = ((text == null) ? (str + " with default reason") : (str + $" with reason: {text}"));
		chatPresenter.SystemMessage(str);
		AddOrUpdateSanction(param, new Sanction(SanctionType.Mute, text), result);
		return true;
	}

	private bool UnsilenceCommand(string contents)
	{
		if (contents.IsNullOrWhiteSpace())
		{
			return false;
		}
		if (contents.Contains(" "))
		{
			return false;
		}
		chatPresenter.SystemMessage("Unsilencing " + contents);
		AddOrUpdateSanction(contents, new Sanction(SanctionType.Mute, string.Empty), 0, remove: true);
		return true;
	}

	private bool SuspendCommand(string contents)
	{
		if (contents.IsNullOrWhiteSpace())
		{
			return false;
		}
		ChatCommands.GetParam(contents, out string param, out string remainder);
		if (param.IsNullOrWhiteSpace())
		{
			return false;
		}
		if (IsMe(param))
		{
			chatPresenter.SystemMessage("You cannot do that to yourself");
			return true;
		}
		if (remainder.IsNullOrWhiteSpace())
		{
			return false;
		}
		int num = remainder.LastIndexOf(" ", StringComparison.Ordinal);
		string s = remainder.Substring(num + 1);
		string text;
		if (num != -1 && int.TryParse(s, out int result))
		{
			if (result < 1)
			{
				chatPresenter.SystemMessage("Cannot suspend for less than 1 day");
				return true;
			}
			text = remainder.Substring(0, num);
		}
		else
		{
			result = -1;
			text = remainder;
		}
		if (text.IsNullOrWhiteSpace())
		{
			return false;
		}
		if (int.TryParse(text, out int _))
		{
			return false;
		}
		string str = $"Suspending {param}";
		str = ((result >= 0) ? (str + $" for {result} day(s)") : (str + " permanently"));
		str += $" with reason: {text}";
		chatPresenter.SystemMessage(str);
		AddOrUpdateSanction(param, new Sanction(SanctionType.Suspension, text), result);
		return true;
	}

	private bool IsMe(string user)
	{
		return User.DisplayName.Equals(user, StringComparison.InvariantCultureIgnoreCase) || User.Username.Equals(user, StringComparison.InvariantCultureIgnoreCase);
	}

	private bool UnsuspendCommand(string contents)
	{
		if (contents.Contains(" "))
		{
			return false;
		}
		chatPresenter.SystemMessage("Un-suspending " + contents);
		AddOrUpdateSanction(contents, new Sanction(SanctionType.Suspension, string.Empty), 0, remove: true);
		return true;
	}

	private bool WarnCommand(string contents)
	{
		ChatCommands.GetParam(contents, out string param, out string remainder);
		if (param.IsNullOrWhiteSpace() || remainder.IsNullOrWhiteSpace())
		{
			return false;
		}
		if (IsMe(param))
		{
			chatPresenter.SystemMessage("You cannot do that to yourself");
			return true;
		}
		chatPresenter.SystemMessage("Warning " + param + " with reason '" + remainder + "'");
		AddOrUpdateSanction(param, new Sanction(SanctionType.Warning, remainder));
		return true;
	}

	private bool NoteCommand(string contents)
	{
		ChatCommands.GetParam(contents, out string param, out string remainder);
		if (param.IsNullOrWhiteSpace() || remainder.IsNullOrWhiteSpace())
		{
			return false;
		}
		if (IsMe(param))
		{
			chatPresenter.SystemMessage("You cannot do that to yourself");
			return true;
		}
		chatPresenter.SystemMessage("Adding note '" + remainder + "' to " + param);
		AddOrUpdateSanction(param, new Sanction(SanctionType.Note, remainder));
		return true;
	}

	private bool RenameOffensiveClanCommand(string input)
	{
		ChatCommands.GetParam(input, out string param, out string remainder);
		if (!SocialInputValidation.ValidateUserName(ref remainder))
		{
			chatPresenter.SystemMessage(Localization.Get("strErrorClanCreationInvalidName", true));
			return true;
		}
		if (remainder.Length > 16)
		{
			chatPresenter.SystemMessage(Localization.Get("strErrorClanCreationInvalidName", true));
			return true;
		}
		ClanRenameDependency dependency = new ClanRenameDependency(param, remainder, User.Username);
		IRenameClanRequest renameClanRequest = socialRequestFactory.Create<IRenameClanRequest>();
		renameClanRequest.Inject(dependency);
		renameClanRequest.SetAnswer(new ServiceAnswer(OnClanRenamed, OnClanRenameFailed));
		renameClanRequest.Execute();
		return true;
	}

	private void OnClanRenamed()
	{
		chatPresenter.SystemMessage("Clan renamed sucessfully");
	}

	private void OnClanRenameFailed(ServiceBehaviour serviceBehaviour)
	{
		if (serviceBehaviour.errorCode == 1)
		{
			ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
			return;
		}
		string bodyText = Localization.Get(((SocialErrorCode)serviceBehaviour.errorCode).ToString(), true);
		GenericErrorData error = new GenericErrorData(serviceBehaviour.errorTitle, bodyText);
		ErrorWindow.ShowErrorWindow(error);
	}

	private void AddOrUpdateSanction(string username, Sanction sanction, int duration = 0, bool remove = false)
	{
		AddOrUpdateSanctionDependency addOrUpdateSanctionDependency = default(AddOrUpdateSanctionDependency);
		addOrUpdateSanctionDependency.UserName = username;
		addOrUpdateSanctionDependency.Sanction = sanction;
		addOrUpdateSanctionDependency.Duration = duration;
		addOrUpdateSanctionDependency.Remove = remove;
		AddOrUpdateSanctionDependency param = addOrUpdateSanctionDependency;
		chatRequestFactory.Create<IAddOrUpdateSanctionRequest, AddOrUpdateSanctionDependency>(param).SetAnswer(new ServiceAnswer(delegate
		{
			SanctionApplied(username, sanction);
		}, OnSanctionFailed)).Execute();
	}

	private void OnSanctionFailed(ServiceBehaviour serviceBehaviour)
	{
		ChatReasonCode errorCode = (ChatReasonCode)serviceBehaviour.errorCode;
		if (errorCode != ChatReasonCode.STR_CHAT_REASON_UNEXPECTED_ERROR)
		{
			chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString(errorCode.ToString()));
		}
		else
		{
			ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
		}
	}

	private void SanctionApplied(string username, Sanction sanction)
	{
		chatPresenter.SystemMessage("Applied successfully");
	}

	private void SanctionReceived(Sanction sanction)
	{
		int? duration = sanction.Duration;
		string key3;
		Action okClicked;
		string key;
		string key2;
		switch (sanction.SanctionType)
		{
		case SanctionType.Warning:
			key = "strWarning";
			key2 = "strAcknowledge";
			key3 = "strWarningDetails";
			okClicked = delegate
			{
				AcknowledgeSanction(sanction);
			};
			break;
		case SanctionType.Mute:
			key = "strSilenced";
			key2 = "strAcknowledge";
			key3 = "strSilencedDetails";
			okClicked = delegate
			{
				AcknowledgeSanction(sanction);
			};
			break;
		case SanctionType.Suspension:
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			disableChatObservable.Dispatch();
			key = "strSuspension";
			key2 = "strQuit";
			key3 = "strSuspensionDetails";
			okClicked = Application.Quit;
			HandleSuspensionEvent();
			break;
		case SanctionType.Kick:
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			disableChatObservable.Dispatch();
			key = "strTencentKickedTitle";
			key2 = "strQuit";
			key3 = "strTencentKickedBody";
			okClicked = delegate
			{
				AcknowledgeSanction(sanction);
				Application.Quit();
			};
			break;
		default:
			throw new Exception("Unhandled case");
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString(key3));
		stringBuilder.Append("\n");
		if (sanction.Reason != string.Empty)
		{
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetReplaceString("strReason", "[REASON]", sanction.Reason));
		}
		if (duration.HasValue)
		{
			stringBuilder.Append(' ');
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetReplaceString("strSanctionDurationSuffix", "[DURATION]", sanction.Duration.Value.ToString()));
		}
		key = StringTableBase<StringTable>.Instance.GetString(key);
		key2 = StringTableBase<StringTable>.Instance.GetString(key2);
		_genericErrorData = new GenericErrorData(key, stringBuilder.ToString(), key2, okClicked);
		if (CanShowSanctionDialog())
		{
			ErrorWindow.ShowErrorWindow(_genericErrorData);
		}
	}

	private void ClanRenamed(ClanRenameDependency clanRenameDependency)
	{
		string bodyText = StringTableBase<StringTable>.Instance.GetString("strClanRenamedNotification").Replace("{ADMINNAME}", clanRenameDependency.AdminName);
		GenericErrorData data = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strWarning"), bodyText);
		GenericInfoDisplay.ShowInfoDialogue(data);
	}

	private void AcknowledgeSanction(Sanction sanction)
	{
		chatRequestFactory.Create<IAcknowledgeSanctionRequest, Sanction>(sanction).Execute();
	}
}
