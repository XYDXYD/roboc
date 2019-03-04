using Fabric;
using Mothership.GUI.Social;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;

namespace Mothership.GUI.Clan
{
	internal sealed class ClanInvitationController
	{
		private bool _focused;

		private ClanInvitationView _view;

		private string _playerName;

		private TextLabelComponentDataContainer _labelDataContainer = new TextLabelComponentDataContainer(string.Empty);

		private ShortCutMode _previousShortCutMode;

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputController inputController
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

		public void SetView(ClanInvitationView view)
		{
			_view = view;
		}

		public void HandleMessage(object message)
		{
			if (!(message is GenericComponentMessage))
			{
				return;
			}
			GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
			switch (genericComponentMessage.Message)
			{
			case MessageType.ButtonWithinListClicked:
			case MessageType.Disable:
			case MessageType.Enable:
			case MessageType.Hide:
				break;
			case MessageType.ButtonClicked:
				if (genericComponentMessage.Originator == "InviteClanButton")
				{
					if (!string.IsNullOrEmpty(_playerName))
					{
						SendInvitationRequest();
					}
					else
					{
						ShowInvalidUsernameError();
					}
				}
				else if (genericComponentMessage.Originator == "CloseClanInvitationButton")
				{
					_view.get_gameObject().SetActive(false);
				}
				else if (genericComponentMessage.Originator == "DismissSuccesfulPopupButton")
				{
					_view.BroadcastDownGenericMessage(new GenericComponentMessage(MessageType.Hide, "ClanInvitationRoot", ClanGUIStrings.CLAN_SUCCESSFUL_INVITE_POPUP));
				}
				break;
			case MessageType.TextEntryChanged:
				if (genericComponentMessage.Originator == "PlayerNameTextField")
				{
					string playerName = genericComponentMessage.Data.UnpackData<string>();
					if (SocialInputValidation.ValidateUserName(ref playerName) || SocialInputValidation.DoesStringContainAtSymbol(ref playerName))
					{
						_playerName = playerName;
					}
					else
					{
						_playerName = string.Empty;
					}
				}
				break;
			case MessageType.TextEntrySubmitted:
				if (genericComponentMessage.Originator == "PlayerNameTextField")
				{
					if (!string.IsNullOrEmpty(_playerName))
					{
						SendInvitationRequest();
					}
					else
					{
						ShowInvalidUsernameError();
					}
				}
				break;
			case MessageType.Show:
				if (genericComponentMessage.Target == "ClanInvitationRoot")
				{
					_view.get_gameObject().SetActive(true);
					_view.BroadcastDownGenericMessage(new GenericComponentMessage(MessageType.SetFocus, "PlayerNameTextField", string.Empty));
				}
				break;
			case MessageType.OnFocus:
				if (genericComponentMessage.Originator == "PlayerNameTextField")
				{
					OnFocusSet(focused: true);
				}
				break;
			case MessageType.OnUnfocus:
				if (genericComponentMessage.Originator == "PlayerNameTextField")
				{
					OnFocusSet(focused: false);
				}
				break;
			}
		}

		private void OnFocusSet(bool focused)
		{
			if (focused != _focused)
			{
				_focused = focused;
				if (focused)
				{
					_previousShortCutMode = inputController.GetShortCutMode();
					inputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
				}
				else
				{
					inputController.SetShortCutMode(_previousShortCutMode);
				}
			}
		}

		private void ShowInvalidUsernameError()
		{
			_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanInviteInvalidUsername"));
			_view.BroadcastDownGenericMessage(new GenericComponentMessage(MessageType.Show, string.Empty, "InviteSomeoneErrorLabel"));
			_view.BroadcastDownGenericMessage(new GenericComponentMessage(MessageType.SetData, "InviteSomeoneErrorLabel", string.Empty, _labelDataContainer));
		}

		private void SendInvitationRequest()
		{
			loadingIconPresenter.NotifyLoading("Clans");
			IInviteToClanRequest inviteToClanRequest = socialRequestFactory.Create<IInviteToClanRequest>();
			inviteToClanRequest.Inject(_playerName);
			inviteToClanRequest.SetAnswer(new ServiceAnswer<ClanMember[]>(OnInviteSent, OnInviteFailed));
			inviteToClanRequest.Execute();
		}

		private void OnInviteSent(ClanMember[] clanMember)
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
			_view.BroadcastDownGenericMessage(new GenericComponentMessage(MessageType.Show, "ClanInvitationRoot", ClanGUIStrings.CLAN_SUCCESSFUL_INVITE_POPUP));
			_view.BubbleUpGenericMessage(new GenericComponentMessage(MessageType.RefreshData, "ClanInvitationRoot", "YourClanRoot"));
		}

		private void OnInviteFailed(ServiceBehaviour serviceBehaviour)
		{
			bool flag = true;
			loadingIconPresenter.NotifyLoadingDone("Clans");
			int errorCode = serviceBehaviour.errorCode;
			SocialErrorCode socialErrorCode = (SocialErrorCode)errorCode;
			switch (socialErrorCode)
			{
			case SocialErrorCode.STR_SOCIAL_REASON_USER_DOES_NOT_EXIST:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanInviteUserNotExists"));
				break;
			case SocialErrorCode.STR_SOCIAL_REASON_AUTO_DECLINED_FRIEND_OR_CLAN:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanInviteUserNoInvites"));
				break;
			case SocialErrorCode.STR_SOCIAL_REASON_TARGET_ALREADY_IN_CLAN:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanInviteUserInClan"));
				break;
			case SocialErrorCode.STR_SOCIAL_REASON_INVITE_ALREADY_SENT:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanInviteInvitationSent"));
				break;
			case SocialErrorCode.STR_SOCIAL_REASON_CLAN_RANK_TOO_LOW:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanInviteUserRankLow"));
				break;
			case SocialErrorCode.STR_SOCIAL_REASON_INVALID_USERNAME:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanInviteInvalidUsername"));
				break;
			case SocialErrorCode.STR_SOCIAL_REASON_USER_BLOCKED_YOU:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString(socialErrorCode.ToString()).Replace("{PLAYER}", _playerName));
				break;
			default:
				flag = false;
				break;
			}
			if (flag)
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[131], 0, (object)null, _view.get_gameObject());
				_view.BroadcastDownGenericMessage(new GenericComponentMessage(MessageType.Show, string.Empty, "InviteSomeoneErrorLabel"));
				_view.BroadcastDownGenericMessage(new GenericComponentMessage(MessageType.SetData, "InviteSomeoneErrorLabel", string.Empty, _labelDataContainer));
			}
		}
	}
}
