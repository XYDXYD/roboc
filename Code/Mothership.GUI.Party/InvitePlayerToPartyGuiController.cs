using Mothership.GUI.CustomGames;
using Mothership.GUI.Social;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership.GUI.Party
{
	internal class InvitePlayerToPartyGuiController : IFloatingWidget
	{
		private string _playerNameInTextField;

		private InvitationToPartyType _invitationToPartyType;

		private InvitePlayerToPartyGuiView _view;

		private BubbleSignal<IPartyGUIViewRoot> _bubbleToPartyRoot;

		private SendInviteToPartyMessage _inviteToPartyMessage = new SendInviteToPartyMessage();

		private SignalChain _signal;

		private GenericComponentMessage _genericComponentMessage;

		private bool _textSubmissionLocked;

		private IDataSource _clanMembersDataSource;

		private IDataSource _friendsDataSource;

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

		public void SetView(InvitePlayerToPartyGuiView view)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			_view = view;
			_signal = new SignalChain(view.get_transform());
			_bubbleToPartyRoot = new BubbleSignal<IPartyGUIViewRoot>(_view.get_transform());
			_genericComponentMessage = new GenericComponentMessage(MessageType.SetFocus, string.Empty, string.Empty);
		}

		public void SetDataSources(IDataSource friendsDataSource, IDataSource clanMembersDataSource)
		{
			_clanMembersDataSource = clanMembersDataSource;
			_friendsDataSource = friendsDataSource;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshPlayersData);
		}

		private IEnumerator RefreshPlayersData()
		{
			yield return _friendsDataSource.RefreshData();
			yield return _clanMembersDataSource.RefreshData();
		}

		public void HandleMessage(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
				switch (genericComponentMessage.Message)
				{
				case MessageType.ButtonClicked:
					if (genericComponentMessage.Originator == "PartyInvitationButton")
					{
						SendInvitationRequest(_playerNameInTextField);
					}
					break;
				case MessageType.TextEntryChanged:
					if (genericComponentMessage.Originator == "PlayerNameTextField")
					{
						_playerNameInTextField = genericComponentMessage.Data.UnpackData<string>();
					}
					break;
				case MessageType.TextEntrySubmitted:
					if (genericComponentMessage.Originator == "PlayerNameTextField" && !_textSubmissionLocked)
					{
						SendInvitationRequest(_playerNameInTextField);
					}
					break;
				}
			}
			else if (message is InvitePlayerMessage)
			{
				InvitePlayerMessage invitePlayerMessage = (InvitePlayerMessage)message;
				SendInvitationRequest(invitePlayerMessage.name);
			}
			else if (message is ShowPartyInviteDropDownMessage)
			{
				ShowPartyInviteDropDownMessage showPartyInviteDropDownMessage = message as ShowPartyInviteDropDownMessage;
				Show(showPartyInviteDropDownMessage.anchorWidget, showPartyInviteDropDownMessage.constrainArea);
				_invitationToPartyType = InvitationToPartyType.RegularMultiplayerParty;
			}
			else if (message is ShowPartyInviteDropDownMessageForCustomGame)
			{
				ShowPartyInviteDropDownMessageForCustomGame showPartyInviteDropDownMessageForCustomGame = message as ShowPartyInviteDropDownMessageForCustomGame;
				Show(showPartyInviteDropDownMessageForCustomGame.anchorWidget, showPartyInviteDropDownMessageForCustomGame.areaWidget);
				if (showPartyInviteDropDownMessageForCustomGame.teamChoice == CustomGameTeamChoice.TeamA)
				{
					_invitationToPartyType = InvitationToPartyType.CustomGameTeamA;
				}
				else
				{
					_invitationToPartyType = InvitationToPartyType.CustomGameTeamB;
				}
			}
			else if (message is SendInviteToPartyResponse)
			{
				SendInviteToPartyResponse sendInviteToPartyResponse = message as SendInviteToPartyResponse;
				_textSubmissionLocked = false;
				if (!sendInviteToPartyResponse.success)
				{
					ShowErrorRaw(sendInviteToPartyResponse.errorMsg);
				}
				else
				{
					Hide();
				}
			}
			else if (message is PartyInvitationReceivedMessage || message is HidePopupMenuMessage)
			{
				Hide();
			}
		}

		private void Hide()
		{
			if (_view.get_gameObject().get_activeSelf())
			{
				_genericComponentMessage.Message = MessageType.SetUnfocus;
				_signal.DeepBroadcast<GenericComponentMessage>(_genericComponentMessage);
				_view.Hide();
				inputController.RemoveFloatingWidget(this);
				inputController.UpdateShortCutMode();
			}
		}

		private void Show(UIWidget anchorWidget, UIWidget constrainArea = null)
		{
			if (_view.get_gameObject().get_activeSelf())
			{
				Hide();
			}
			_view.ShowNoErrorMsg();
			_view.Show(anchorWidget, constrainArea);
			_genericComponentMessage.Message = MessageType.SetFocus;
			_signal.DeepBroadcast<GenericComponentMessage>(_genericComponentMessage);
			inputController.SetShortCutMode(ShortCutMode.OnlyEsc);
			inputController.AddFloatingWidget(this);
			TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshPlayersData);
		}

		public void Tick()
		{
			if (Input.GetMouseButtonDown(0))
			{
				GameObject hoveredObject = UICamera.get_hoveredObject();
				if (hoveredObject == null || !SocialStaticUtilities.IsParentOf(_view.get_transform().get_parent(), hoveredObject.get_transform()))
				{
					Hide();
				}
			}
		}

		private void SendInvitationRequest(string playerName)
		{
			_textSubmissionLocked = true;
			_inviteToPartyMessage.invitee = playerName;
			_inviteToPartyMessage.InvitationPartyType = _invitationToPartyType;
			_bubbleToPartyRoot.TargetedDispatch<SendInviteToPartyMessage>(_inviteToPartyMessage);
		}

		private void ShowErrorRaw(string rawMessageText)
		{
			_view.DisplayErrorMsg(rawMessageText);
		}

		public void HandleQuitPressed()
		{
			Hide();
		}
	}
}
