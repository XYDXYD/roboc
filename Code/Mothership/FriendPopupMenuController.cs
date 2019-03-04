using Mothership.GUI.Social;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.UI.Comms.SignalChain;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal class FriendPopupMenuController : GenericPopupMenuController
	{
		private const string KICK_FRIEND_ACTION = "KickFriend";

		private const string ADD_TO_PARTY_ACTION = "AddToParty";

		private const string CONNFIRMATION_DIALOG_CONFIRM = "ConfirmationDialogConfirm";

		private const string CONNFIRMATION_DIALOG_CANCEL = "ConfirmationDialogCancel";

		private bool _pendingInvite;

		private Friend _clickedFriend;

		private string _currentSelectedAction;

		private BubbleSignal<ISocialRoot> _bubble;

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
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
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal FriendController friendController
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

		internal void ShowPopupMenuForContext(Friend friend)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = UICamera.currentCamera.ViewportToWorldPoint(new Vector3(Mathf.Clamp01(UICamera.currentTouch.pos.x / (float)Screen.get_width()), Mathf.Clamp01(UICamera.currentTouch.pos.y / (float)Screen.get_height()), 0f));
			PositionUnAnchoredMenu(position);
			_clickedFriend = friend;
			ResetMenu();
			loadingIconPresenter.NotifyLoading("QueryPlatoonStatus");
			socialRequestFactory.Create<IGetPlatoonPendingInviteRequest>().SetAnswer(new ServiceAnswer<PlatoonInvite>(OnGotPendingInvites, OnServiceFail)).Execute();
		}

		internal override void SetView(GenericPopupMenuView view)
		{
			_bubble = new BubbleSignal<ISocialRoot>(view.get_transform());
			base.SetView(view);
		}

		private void OnServiceFail(ServiceBehaviour obj)
		{
			loadingIconPresenter.NotifyLoadingDone("QueryPlatoonStatus");
			AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strRemoveFriend"), "KickFriend");
			Show();
		}

		private void OnGotPendingInvites(PlatoonInvite invite)
		{
			_pendingInvite = (invite != null);
			socialRequestFactory.Create<IGetPlatoonDataRequest>().SetAnswer(new ServiceAnswer<Platoon>(OnGotPlatoonData, OnServiceFail)).Execute();
		}

		private void OnGotPlatoonData(Platoon platoon)
		{
			loadingIconPresenter.NotifyLoadingDone("QueryPlatoonStatus");
			if (!_pendingInvite && _clickedFriend.IsOnline && guiInputController.GetActiveScreen() != GuiScreens.BattleCountdown && (platoon.Size <= 0 || (!platoon.HasPlayer(_clickedFriend.Name) && platoon.GetIsPlatoonLeader())))
			{
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strInviteToPlatoon"), "AddToParty");
			}
			AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strRemoveFriend"), "KickFriend");
			Show();
		}

		internal override void HandleMessage(GenericComponentMessage message)
		{
			string originator = message.Originator;
			if (originator == null)
			{
				return;
			}
			if (!(originator == "AddToParty"))
			{
				if (!(originator == "KickFriend"))
				{
					if (!(originator == "ConfirmationDialogConfirm"))
					{
						if (originator == "ConfirmationDialogCancel")
						{
							HideConfirmationDialog();
							_view.Hide();
							_currentSelectedAction = string.Empty;
						}
					}
					else
					{
						HideConfirmationDialog();
						_view.Hide();
						ExecuteCurrentSelectedAction();
					}
				}
				else
				{
					_currentSelectedAction = "KickFriend";
					ShowConfirmationDialog();
				}
			}
			else
			{
				InviteToParty(_clickedFriend);
				_view.Hide();
			}
		}

		private void ExecuteCurrentSelectedAction()
		{
			string currentSelectedAction = _currentSelectedAction;
			if (currentSelectedAction != null && currentSelectedAction == "KickFriend")
			{
				SendRemoveFriendRequest(_clickedFriend);
			}
		}

		private void ShowConfirmationDialog()
		{
			_view.DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "ClanPopupMenuRoot", "ConfirmationDialog"));
		}

		private void HideConfirmationDialog()
		{
			_view.DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "ClanPopupMenuRoot", "ConfirmationDialog"));
		}

		public void SendRemoveFriendRequest(Friend friend)
		{
			loadingIconPresenter.NotifyLoading("Friends");
			IServiceRequest serviceRequest = socialRequestFactory.Create<IRemoveFriendRequest, string>(friend.Name).SetAnswer(new ServiceAnswer<IList<Friend>>(delegate(IList<Friend> friendList)
			{
				loadingIconPresenter.NotifyLoadingDone("Friends");
				friendController.DispatchAnyFriendMessage(new SocialMessage(SocialMessageType.Refresh, string.Empty, friendList));
			}, OnError));
			serviceRequest.Execute();
		}

		private void InviteToParty(Friend friend)
		{
			_bubble.TargetedDispatch<SocialScreenQuickInviteToPartyMessage>(new SocialScreenQuickInviteToPartyMessage(friend.Name));
		}

		private void OnError(ServiceBehaviour serviceBehaviour)
		{
			SocialErrorCode errorCode = (SocialErrorCode)serviceBehaviour.errorCode;
			if (errorCode == SocialErrorCode.STR_SOCIAL_REASON_UNEXPECTED_ERROR)
			{
				serviceBehaviour.SetAlternativeBehaviour(delegate
				{
				}, StringTableBase<StringTable>.Instance.GetString("strCancel"));
				ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
			}
			else
			{
				GenericErrorData error = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strUnexpectedError"), errorCode.ToString());
				ErrorWindow.ShowErrorWindow(error);
			}
		}
	}
}
