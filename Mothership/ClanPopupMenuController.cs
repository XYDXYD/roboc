using Authentication;
using Mothership.GUI.Social;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal class ClanPopupMenuController : GenericPopupMenuController
	{
		private const string PROMOTE_ACTION = "Promote";

		private const string DEMOTE_ACTION = "Demote";

		private const string KICK_ACTION = "Kick";

		private const string KICK_INVITEE_ACTION = "KickInvitee";

		private const string CONNFIRMATION_DIALOG_CONFIRM = "ConfirmationDialogConfirm";

		private const string CONNFIRMATION_DIALOG_CANCEL = "ConfirmationDialogCancel";

		private const string ADD_TO_PARTY_ACTION = "AddToParty";

		private string _clickedPlayersName = string.Empty;

		private string _currentSelectedAction = string.Empty;

		private ClanMemberRank _clickedPlayersRank;

		private ClanPopupMenuContext _menuContext;

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
		internal ClanController clanController
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

		public void ShowPopupMenuForContext(string playerName, ClanMemberRank myRankInClan, ClanMemberRank clickedMembersRank, bool clickedMemberWasInvited, bool clickedMemberWasOnline)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = UICamera.currentCamera.ViewportToWorldPoint(new Vector3(Mathf.Clamp01(UICamera.currentTouch.pos.x / (float)Screen.get_width()), Mathf.Clamp01(UICamera.currentTouch.pos.y / (float)Screen.get_height()), 0f));
			PositionUnAnchoredMenu(position);
			_clickedPlayersName = playerName;
			_clickedPlayersRank = clickedMembersRank;
			ResetMenu();
			_menuContext = ClanPopupMenuContext.MemberForMember;
			switch (myRankInClan)
			{
			case ClanMemberRank.Leader:
				if (clickedMemberWasInvited)
				{
					_menuContext = ClanPopupMenuContext.LeaderContextForInvitee;
					break;
				}
				switch (clickedMembersRank)
				{
				case ClanMemberRank.Leader:
					_menuContext = ClanPopupMenuContext.LeaderContextForSelf;
					break;
				case ClanMemberRank.Member:
					_menuContext = ClanPopupMenuContext.LeaderContextForMember;
					break;
				case ClanMemberRank.Officer:
					_menuContext = ClanPopupMenuContext.LeaderContextForOfficer;
					break;
				}
				break;
			case ClanMemberRank.Officer:
				if (clickedMemberWasInvited)
				{
					_menuContext = ClanPopupMenuContext.OfficerContextForInvitee;
					break;
				}
				switch (clickedMembersRank)
				{
				case ClanMemberRank.Officer:
					_menuContext = ClanPopupMenuContext.OfficerContextForOfficer;
					break;
				case ClanMemberRank.Member:
					_menuContext = ClanPopupMenuContext.OfficerContextForMember;
					break;
				case ClanMemberRank.Leader:
					_menuContext = ClanPopupMenuContext.OfficerContextForLeader;
					break;
				}
				break;
			default:
				_menuContext = ClanPopupMenuContext.MemberForMember;
				break;
			}
			if (clickedMemberWasOnline && !playerName.Equals(User.Username))
			{
				loadingIconPresenter.NotifyLoading("QueryPlatoonStatus");
				socialRequestFactory.Create<IGetPlatoonDataRequest>().SetAnswer(new ServiceAnswer<Platoon>(delegate(Platoon platoon)
				{
					OnGotPlatoonData(platoon, clickedMemberWasOnline, playerName);
				}, OnServiceFail)).Execute();
			}
			else
			{
				ShowPopupMenu(includeAddToParty: false);
			}
		}

		private void OnServiceFail(ServiceBehaviour obj)
		{
			loadingIconPresenter.NotifyLoadingDone("QueryPlatoonStatus");
			ShowPopupMenu(includeAddToParty: false);
		}

		private void OnGotPlatoonData(Platoon platoon, bool clickedPlayerOnline, string clickedPlayerName)
		{
			if (clickedPlayerOnline && (platoon.Size <= 0 || (!platoon.HasPlayer(clickedPlayerName) && platoon.GetIsPlatoonLeader())))
			{
				socialRequestFactory.Create<IGetPlatoonPendingInviteRequest>().SetAnswer(new ServiceAnswer<PlatoonInvite>(OnGotPendingInvites, OnServiceFail)).Execute();
				return;
			}
			loadingIconPresenter.NotifyLoadingDone("QueryPlatoonStatus");
			ShowPopupMenu(includeAddToParty: false);
		}

		private void OnGotPendingInvites(PlatoonInvite invite)
		{
			loadingIconPresenter.NotifyLoadingDone("QueryPlatoonStatus");
			if (invite == null && guiInputController.GetActiveScreen() != GuiScreens.BattleCountdown)
			{
				ShowPopupMenu(includeAddToParty: true);
			}
			else
			{
				ShowPopupMenu(includeAddToParty: false);
			}
		}

		public void ShowPopupMenu(bool includeAddToParty)
		{
			if (includeAddToParty)
			{
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strInviteToPlatoon"), "AddToParty");
			}
			switch (_menuContext)
			{
			case ClanPopupMenuContext.LeaderContextForOfficer:
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strYourClanPopupMenuItemPromoteLeader"), "Promote");
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strYourClanPopupMenuItemDemoteToMember"), "Demote");
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strYourClanPopupMenuItemKickFromClan"), "Kick");
				Show();
				break;
			case ClanPopupMenuContext.LeaderContextForMember:
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strYourClanPopupMenuItemPromoteOfficer"), "Promote");
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strYourClanPopupMenuItemKickFromClan"), "Kick");
				Show();
				break;
			case ClanPopupMenuContext.LeaderContextForInvitee:
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strYourClanPopupMenuItemKickFromClan"), "KickInvitee");
				Show();
				break;
			case ClanPopupMenuContext.OfficerContextForMember:
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strYourClanPopupMenuItemKickFromClan"), "Kick");
				Show();
				break;
			case ClanPopupMenuContext.OfficerContextForInvitee:
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strYourClanPopupMenuItemKickFromClan"), "KickInvitee");
				Show();
				break;
			default:
				if (includeAddToParty)
				{
					Show();
				}
				break;
			}
		}

		internal override void HandleMessage(GenericComponentMessage message)
		{
			switch (message.Originator)
			{
			case "Promote":
				_currentSelectedAction = "Promote";
				ShowConfirmationDialog();
				break;
			case "Demote":
				_currentSelectedAction = "Demote";
				ShowConfirmationDialog();
				break;
			case "Kick":
				_currentSelectedAction = "Kick";
				ShowConfirmationDialog();
				break;
			case "KickInvitee":
				_currentSelectedAction = "KickInvitee";
				ShowConfirmationDialog();
				break;
			case "ConfirmationDialogConfirm":
				HideConfirmationDialog();
				_view.Hide();
				ExecuteCurrentSelectedAction();
				break;
			case "ConfirmationDialogCancel":
				HideConfirmationDialog();
				_view.Hide();
				_currentSelectedAction = string.Empty;
				break;
			case "AddToParty":
				InviteToParty(_clickedPlayersName);
				_view.Hide();
				break;
			}
		}

		private void ExecuteCurrentSelectedAction()
		{
			string currentSelectedAction = _currentSelectedAction;
			if (currentSelectedAction == null)
			{
				return;
			}
			if (!(currentSelectedAction == "Promote"))
			{
				if (!(currentSelectedAction == "Demote"))
				{
					if (!(currentSelectedAction == "Kick"))
					{
						if (currentSelectedAction == "KickInvitee")
						{
							SendKickInviteeRequest();
						}
					}
					else
					{
						SendKickPlayerRequest();
					}
				}
				else
				{
					SendChangePlayerRankRequest(GetPlayersProposedNewRank(promotion: false));
				}
			}
			else
			{
				SendChangePlayerRankRequest(GetPlayersProposedNewRank(promotion: true));
			}
		}

		private void InviteToParty(string playerName)
		{
			_bubble.TargetedDispatch<SocialScreenQuickInviteToPartyMessage>(new SocialScreenQuickInviteToPartyMessage(playerName));
		}

		private void ShowConfirmationDialog()
		{
			_view.DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "ClanPopupMenuRoot", "ConfirmationDialog"));
		}

		private void HideConfirmationDialog()
		{
			_view.DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "ClanPopupMenuRoot", "ConfirmationDialog"));
		}

		private ClanMemberRank GetPlayersProposedNewRank(bool promotion)
		{
			ClanMemberRank result = _clickedPlayersRank;
			if (_clickedPlayersRank == ClanMemberRank.Member && promotion)
			{
				result = ClanMemberRank.Officer;
			}
			else if (_clickedPlayersRank == ClanMemberRank.Officer)
			{
				result = (promotion ? ClanMemberRank.Leader : ClanMemberRank.Member);
			}
			else if (_clickedPlayersRank == ClanMemberRank.Leader && !promotion)
			{
				result = ClanMemberRank.Officer;
			}
			return result;
		}

		private void SendChangePlayerRankRequest(ClanMemberRank newRank)
		{
			loadingIconPresenter.NotifyLoading("Clans");
			IChangeClanMemberRankRequest changeClanMemberRankRequest = socialRequestFactory.Create<IChangeClanMemberRankRequest>();
			changeClanMemberRankRequest.Inject(new ChangeClanMemberRankDependency(_clickedPlayersName, newRank));
			changeClanMemberRankRequest.SetAnswer(new ServiceAnswer<ClanMember[]>(delegate
			{
				loadingIconPresenter.NotifyLoadingDone("Clans");
				HandleChangePlayerRankSuccess();
			}, HandleClanActionError)).Execute();
		}

		private void SendKickPlayerRequest()
		{
			loadingIconPresenter.NotifyLoading("Clans");
			IRemoveFromClanRequest removeFromClanRequest = socialRequestFactory.Create<IRemoveFromClanRequest>();
			removeFromClanRequest.Inject(_clickedPlayersName);
			removeFromClanRequest.SetAnswer(new ServiceAnswer<ClanMember[]>(delegate
			{
				loadingIconPresenter.NotifyLoadingDone("Clans");
				HandleRemoveFromClanSuccess();
			}, HandleClanActionError)).Execute();
		}

		private void SendKickInviteeRequest()
		{
			loadingIconPresenter.NotifyLoading("Clans");
			ICancelClanInviteRequest cancelClanInviteRequest = socialRequestFactory.Create<ICancelClanInviteRequest>();
			cancelClanInviteRequest.Inject(_clickedPlayersName);
			cancelClanInviteRequest.SetAnswer(new ServiceAnswer(delegate
			{
				loadingIconPresenter.NotifyLoadingDone("Clans");
				HandleRemoveInviteeFromClanSuccess();
			}, HandleClanActionError)).Execute();
		}

		private void HandleRemoveInviteeFromClanSuccess()
		{
			clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.Refresh, string.Empty));
		}

		private void HandleRemoveFromClanSuccess()
		{
			clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.Refresh, string.Empty));
		}

		private void HandleChangePlayerRankSuccess()
		{
			clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.Refresh, string.Empty));
		}

		private void HandleClanActionError(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
			string bodyText = behaviour.errorBody;
			if (behaviour.errorCode == 23)
			{
				bodyText = Localization.Get("STR_SOCIAL_REASON_NOT_CLAN_LEADER", true);
			}
			GenericErrorData error = new GenericErrorData(behaviour.errorTitle, bodyText, Localization.Get("strOK", true));
			ErrorWindow.ShowErrorWindow(error);
		}

		internal override void SetView(GenericPopupMenuView view)
		{
			base.SetView(view);
			_bubble = new BubbleSignal<ISocialRoot>(view.get_transform());
		}
	}
}
