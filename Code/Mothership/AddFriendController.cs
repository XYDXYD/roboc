using Authentication;
using Robocraft.GUI;
using Services.Analytics;
using SocialServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership
{
	internal class AddFriendController
	{
		private TextLabelComponentDataContainer _labelDataContainer = new TextLabelComponentDataContainer(string.Empty);

		private string _playerName;

		private ShortCutMode _previousShortCutMode;

		private AddFriendView _view;

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

		[Inject]
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public void SetView(AddFriendView view)
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
				if (genericComponentMessage.Originator == "FriendInvitationButton")
				{
					SendInvitationRequest();
				}
				else if (genericComponentMessage.Originator == "CloseFriendInvitationButton")
				{
					_view.get_gameObject().SetActive(false);
				}
				else if (genericComponentMessage.Originator == "DismissSuccessfulPopupButton")
				{
					_view.BroadcastDownGenericMessage(new GenericComponentMessage(MessageType.Hide, "FriendInvitationRoot", "FriendSuccesfulInvitePopUp"));
				}
				break;
			case MessageType.TextEntryChanged:
				if (genericComponentMessage.Originator == "PlayerNameTextField")
				{
					_playerName = genericComponentMessage.Data.UnpackData<string>();
				}
				break;
			case MessageType.TextEntrySubmitted:
				if (genericComponentMessage.Originator == "PlayerNameTextField")
				{
					SendInvitationRequest();
				}
				break;
			case MessageType.Show:
				if (genericComponentMessage.Target == "FriendInvitationRoot")
				{
					_view.get_gameObject().SetActive(true);
					_view.BroadcastDownGenericMessage(new GenericComponentMessage(MessageType.SetFocus, "PlayerNameTextField", string.Empty));
				}
				break;
			case MessageType.OnFocus:
				if (genericComponentMessage.Originator == "PlayerNameTextField")
				{
					_previousShortCutMode = inputController.GetShortCutMode();
					inputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
				}
				break;
			case MessageType.OnUnfocus:
				if (genericComponentMessage.Originator == "PlayerNameTextField")
				{
					inputController.SetShortCutMode(_previousShortCutMode);
				}
				break;
			}
		}

		private void SendInvitationRequest()
		{
			if (!SocialInputValidation.DoesStringContainAtSymbol(ref _playerName) && !SocialInputValidation.ValidateUserName(ref _playerName))
			{
				ShowError(SocialErrorCode.STR_SOCIAL_REASON_INVALID_USERNAME.ToString());
				return;
			}
			if (IsMe(_playerName))
			{
				ShowError(SocialErrorCode.STR_SOCIAL_REASON_USER_IS_SELF.ToString());
				return;
			}
			loadingIconPresenter.NotifyLoading("Friends");
			IServiceRequest serviceRequest = socialRequestFactory.Create<IInviteFriendRequest, string>(_playerName).SetAnswer(new ServiceAnswer<IList<Friend>>(OnInviteSent, OnInviteFailed));
			serviceRequest.Execute();
		}

		private void OnInviteSent(IList<Friend> friendsList)
		{
			loadingIconPresenter.NotifyLoadingDone("Friends");
			_view.BroadcastDownGenericMessage(new GenericComponentMessage(MessageType.Show, "FriendInvitationRoot", "FriendSuccesfulInvitePopUp"));
			_view.BubbleUpGenericMessage(new GenericComponentMessage(MessageType.RefreshData, "FriendInvitationRoot", "FriendListRoot"));
			TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleAnalytics);
		}

		private void ShowError(string localisationKey)
		{
			string rawMessageText = StringTableBase<StringTable>.Instance.GetString(localisationKey).Replace("{PLAYER}", _playerName);
			ShowErrorRaw(rawMessageText);
		}

		private void ShowErrorRaw(string rawMessageText)
		{
			_labelDataContainer.PackData(rawMessageText);
			_view.BroadcastDownGenericMessage(new GenericComponentMessage(MessageType.SetData, "FriendGUIStringErrorLabel", string.Empty, _labelDataContainer));
			_view.BroadcastDownGenericMessage(new GenericComponentMessage(MessageType.Show, string.Empty, "FriendGUIStringErrorLabel"));
		}

		private void OnInviteFailed(ServiceBehaviour serviceBehaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("Friends");
			SocialErrorCode errorCode = (SocialErrorCode)serviceBehaviour.errorCode;
			if (errorCode == SocialErrorCode.STR_SOCIAL_REASON_UNEXPECTED_ERROR)
			{
				RemoteLogger.Error(serviceBehaviour.errorTitle, serviceBehaviour.errorBody, null);
			}
			ShowError(errorCode.ToString());
		}

		private static bool IsMe(string name)
		{
			string myName = GetMyName();
			return string.Compare(name, myName, ignoreCase: true) == 0;
		}

		private static string GetMyName()
		{
			return User.Username;
		}

		private IEnumerator HandleAnalytics()
		{
			TaskService logFriendInvitedRequest = analyticsRequestFactory.Create<ILogFriendInvitedRequest>().AsTask();
			yield return logFriendInvitedRequest;
			if (!logFriendInvitedRequest.succeeded)
			{
				throw new Exception("Log Friend Invited Request failed", logFriendInvitedRequest.behaviour.exceptionThrown);
			}
		}
	}
}
