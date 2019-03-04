using Authentication;
using Mothership.GUI.Party;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership.GUI.Social
{
	internal sealed class SocialGUIController : IInitialize, IWaitForFrameworkDestruction, IWaitForFrameworkInitialization
	{
		private const int FRAMES_BEFORE_POP_CURSOR = 10;

		private PartyGUIView _partyGUI;

		private ITaskRoutine _popCursorModeTask;

		private SignalChain _receivedPartyInviteSignal;

		private IServiceEventContainer _socialEventContainer;

		private SocialGUIView _view;

		private bool _isFocused;

		[Inject]
		internal IGUIInputController guiInputController
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
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ISocialEventContainerFactory socialEventContainerFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ChatCommands chatCommands
		{
			private get;
			set;
		}

		[Inject]
		internal ICursorMode cursorMode
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			guiInputController.OnScreenStateChange += UpdateVisibility;
		}

		public void OnFrameworkDestroyed()
		{
			_socialEventContainer.Dispose();
		}

		public void OnFrameworkInitialized()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			_receivedPartyInviteSignal = new SignalChain(_view.get_transform());
			_socialEventContainer = socialEventContainerFactory.Create();
			_socialEventContainer.ListenTo<IPlatoonInviteEventListener, PlatoonInvite>(HandleOnInvitedToParty);
			_socialEventContainer.ListenTo<IPlatoonInviteCancelledEventListener, string>(HandleOnInviteToPartyCanceled);
			_socialEventContainer.ListenTo<IPlatoonMemberStatusChangedEventListener, string, PlatoonStatusChangedData>(HandleOnMemberStatusChanged);
			_popCursorModeTask = TaskRunner.get_Instance().AllocateNewTaskRoutine();
			CheckForPendingPartyInvitation();
		}

		public void RegisterToContextNotifier(IContextNotifer contextNotifier)
		{
			contextNotifier.AddFrameworkInitializationListener(this);
			contextNotifier.AddFrameworkDestructionListener(this);
		}

		public void SetView(SocialGUIView view)
		{
			_view = view;
		}

		public void HandleMessage(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
				if (genericComponentMessage.Message == MessageType.ButtonClicked)
				{
					if (genericComponentMessage.Originator == "OpenClanScreenButton")
					{
						_view.DeepBroadcast(new SocialMessage(SocialMessageType.MaximizeClanScreen, string.Empty));
					}
					if (genericComponentMessage.Originator == "OpenFriendScreenButton")
					{
						_view.DeepBroadcast(new SocialMessage(SocialMessageType.MaximizeFriendScreen, string.Empty));
					}
				}
			}
			else if (message is ChatGUIEvent)
			{
				ChatGUIEvent chatGUIEvent = (ChatGUIEvent)message;
				if (chatGUIEvent.type == ChatGUIEvent.Type.Focus)
				{
					_view.DeepBroadcast(new SocialMessage(SocialMessageType.CloseSocialScreens, "ChatRoot"));
					_isFocused = true;
				}
				else if (chatGUIEvent.type == ChatGUIEvent.Type.Unfocus)
				{
					_isFocused = false;
				}
			}
			else if (message is SocialMessage)
			{
				SocialMessage socialMessage = message as SocialMessage;
				if (socialMessage.messageDispatched == SocialMessageType.ClanJoined || socialMessage.messageDispatched == SocialMessageType.ClanLeft || socialMessage.messageDispatched == SocialMessageType.ClanCreated)
				{
					_view.DeepBroadcast(new GenericComponentMessage(MessageType.RefreshData, "SocialRoot", "ClanNotificationsBoxRoot"));
				}
			}
			else if (message is SocialScreenQuickInviteToPartyMessage)
			{
				SocialScreenQuickInviteToPartyMessage socialScreenQuickInviteToPartyMessage = message as SocialScreenQuickInviteToPartyMessage;
				InviteToParty(socialScreenQuickInviteToPartyMessage.inviteeName);
			}
		}

		private void UpdateVisibility()
		{
			if (guiInputController.ShouldShowSocialGUI())
			{
				_view.get_gameObject().SetActive(true);
				_view.DeepBroadcast(new SocialMessage(SocialMessageType.SocialViewEnabled, string.Empty));
			}
			else
			{
				_view.DeepBroadcast(new SocialMessage(SocialMessageType.SocialViewDisabled, string.Empty));
				_view.get_gameObject().SetActive(false);
			}
		}

		internal void Tick()
		{
			if (_isFocused && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
			{
				GameObject hoveredObject = UICamera.get_hoveredObject();
				if (hoveredObject == null || (!BelongsToSocial(hoveredObject) && !BelongsToParty(hoveredObject)))
				{
					_view.DeepBroadcast(new SocialMessage(SocialMessageType.ClickedOutsideSocial, string.Empty));
					LaunchPopCursorModeTask();
				}
			}
			else if ((_isFocused && InputRemapper.Instance.GetButtonDown("ToggleChatActive")) || InputRemapper.Instance.GetButtonDown("Quit"))
			{
				LaunchPopCursorModeTask();
			}
		}

		private void LaunchPopCursorModeTask()
		{
			if (_popCursorModeTask != null)
			{
				_popCursorModeTask.SetEnumerator(PopCursorModeIfClickNotInSocialOrParty());
				_popCursorModeTask.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		private IEnumerator PopCursorModeIfClickNotInSocialOrParty()
		{
			for (int i = 0; i < 10; i++)
			{
				yield return null;
			}
			if (guiInputController.GetShortCutMode() == ShortCutMode.BuildShortCuts)
			{
				_view.DeepBroadcast(new SocialMessage(SocialMessageType.ClickedOutsideSocial, string.Empty));
				PartyAndSocialGUIEditModeCursorSwitch.PopCursorModeIfClickNotInSocialOrParty(cursorMode);
			}
		}

		private bool BelongsToSocial(GameObject obj)
		{
			if (obj.GetComponent<DefaultAvatarButton>() != null || obj.GetComponentInParent<AvatarSelectionView>() != null)
			{
				return true;
			}
			return SocialStaticUtilities.IsParentOf(_view.get_transform().get_parent(), obj.get_transform());
		}

		private bool BelongsToParty(GameObject obj)
		{
			if (WorldSwitching.IsInBuildMode())
			{
				if (_partyGUI == null)
				{
					_partyGUI = Object.FindObjectOfType<PartyGUIView>();
				}
				return SocialStaticUtilities.IsParentOf(_partyGUI.get_transform(), obj.get_transform());
			}
			return false;
		}

		private void InviteToParty(string inviteeName)
		{
			commandFactory.Build<InviteToPartyCommand>().Inject(inviteeName).Execute();
		}

		private void HandleOnInvitedToParty(PlatoonInvite platoonInvite)
		{
			_receivedPartyInviteSignal.DeepBroadcast<SocialReceivedPartyInviteMessage>(new SocialReceivedPartyInviteMessage(pendingInvite: true));
		}

		private void HandleOnInviteToPartyCanceled(string inviter)
		{
			if (!inviter.Equals(User.Username, StringComparison.InvariantCultureIgnoreCase))
			{
				_receivedPartyInviteSignal.DeepBroadcast<SocialReceivedPartyInviteMessage>(new SocialReceivedPartyInviteMessage(pendingInvite: false));
			}
		}

		private void HandleOnMemberStatusChanged(string playerDisplayName, PlatoonStatusChangedData statusChangedData)
		{
			if (playerDisplayName.Equals(User.DisplayName, StringComparison.InvariantCultureIgnoreCase) && statusChangedData.newStatus == PlatoonMember.MemberStatus.Ready && statusChangedData.oldStatus == PlatoonMember.MemberStatus.Invited)
			{
				_receivedPartyInviteSignal.DeepBroadcast<SocialReceivedPartyInviteMessage>(new SocialReceivedPartyInviteMessage(pendingInvite: false));
			}
		}

		private void CheckForPendingPartyInvitation()
		{
			loadingIconPresenter.NotifyLoading("CheckPendingInvitation");
			socialRequestFactory.Create<IGetPlatoonPendingInviteRequest>().SetAnswer(new ServiceAnswer<PlatoonInvite>(delegate(PlatoonInvite invite)
			{
				_receivedPartyInviteSignal.DeepBroadcast<SocialReceivedPartyInviteMessage>(new SocialReceivedPartyInviteMessage(invite != null));
				loadingIconPresenter.NotifyLoadingDone("CheckPendingInvitation");
			}, delegate(ServiceBehaviour ServiceBehaviour)
			{
				loadingIconPresenter.NotifyLoadingDone("CheckPendingInvitation");
				ShowPopupError(ServiceBehaviour);
			})).Execute();
		}

		private void ShowPopupError(ServiceBehaviour serviceBehaviour)
		{
			if (serviceBehaviour.errorCode != 39)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(serviceBehaviour.errorTitle, serviceBehaviour.errorBody));
			}
		}
	}
}
