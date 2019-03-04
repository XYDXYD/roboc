using Robocraft.GUI;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Party
{
	internal class PartyPopupMenuController : GenericPopupMenuController
	{
		private const string KICK_FROM_PARTY_ACTION = "KickFromParty";

		private const string CANCEL_PENDING_INVITE = "CancelPendingInvite";

		private const string LEAVE_PARTY = "LeaveParty";

		private const string CHANGE_AVATAR = "ChangeAvatar";

		private string _clickedMember;

		private BubbleSignal<PartyGUIView> _bubble;

		private PartyPopupMenuClickMessage _leavePartyMessage;

		private PartyPopupMenuClickMessage _changeAvatarMessage;

		[Inject]
		internal IGUIInputController inputController
		{
			private get;
			set;
		}

		private void ShowPopupMenuForContext(UIWidget anchorPoint, PartyPopupMenuItems menuItems)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			float x = anchorPoint.get_worldCorners()[0].x;
			Vector3 worldCenter = anchorPoint.get_worldCenter();
			Vector3 position = default(Vector3);
			position._002Ector(x, worldCenter.y, 0f);
			PositionUnAnchoredMenu(position, mirror: true);
			ResetMenu();
			if (GenericPopupMenuController.isFlagSet(PartyPopupMenuItems.CancelPendingInvitation, menuItems))
			{
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strCancelPartyInvite"), "CancelPendingInvite");
			}
			if (GenericPopupMenuController.isFlagSet(PartyPopupMenuItems.RemoveFromParty, menuItems))
			{
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strRemoveFromPlatoon"), "KickFromParty");
			}
			if (GenericPopupMenuController.isFlagSet(PartyPopupMenuItems.ChangeAvatar, menuItems))
			{
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strChangeAvatar"), "ChangeAvatar");
			}
			if (GenericPopupMenuController.isFlagSet(PartyPopupMenuItems.LeaveParty, menuItems))
			{
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strLeavePlatoon"), "LeaveParty");
			}
			Show();
		}

		internal override void SetView(GenericPopupMenuView view)
		{
			_bubble = new BubbleSignal<PartyGUIView>(view.get_transform());
			base.SetView(view);
			Hide();
			_leavePartyMessage = new PartyPopupMenuClickMessage(string.Empty, PartyPopupMenuItems.LeaveParty);
			_changeAvatarMessage = new PartyPopupMenuClickMessage(string.Empty, PartyPopupMenuItems.ChangeAvatar);
		}

		internal override void HandleMessage(GenericComponentMessage message)
		{
			string originator = message.Originator;
			if (originator == null)
			{
				return;
			}
			if (!(originator == "KickFromParty"))
			{
				if (!(originator == "CancelPendingInvite"))
				{
					if (!(originator == "LeaveParty"))
					{
						if (originator == "ChangeAvatar")
						{
							Hide();
							_bubble.TargetedDispatch<PartyPopupMenuClickMessage>(_changeAvatarMessage);
						}
					}
					else
					{
						Hide();
						_bubble.TargetedDispatch<PartyPopupMenuClickMessage>(_leavePartyMessage);
					}
				}
				else
				{
					Hide();
					_bubble.TargetedDispatch<PartyPopupMenuClickMessage>(new PartyPopupMenuClickMessage(_clickedMember, PartyPopupMenuItems.CancelPendingInvitation));
				}
			}
			else
			{
				Hide();
				_bubble.TargetedDispatch<PartyPopupMenuClickMessage>(new PartyPopupMenuClickMessage(_clickedMember, PartyPopupMenuItems.RemoveFromParty));
			}
		}

		internal override void Listen(object message)
		{
			base.Listen(message);
			if (message.GetType() == typeof(ShowPartyPopupMenuMessage))
			{
				ShowPartyPopupMenuMessage showPartyPopupMenuMessage = (ShowPartyPopupMenuMessage)message;
				_clickedMember = showPartyPopupMenuMessage.playerName;
				ShowPopupMenuForContext(showPartyPopupMenuMessage.anchorWidget, showPartyPopupMenuMessage.availableOptions);
			}
		}

		protected override void Show()
		{
			inputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			base.Show();
		}

		public override void Hide()
		{
			if (_view.get_gameObject().get_activeSelf())
			{
				inputController.UpdateShortCutMode();
			}
			base.Hide();
		}

		internal override void TickWhileVisible()
		{
			if (InputRemapper.Instance.GetButtonDown("Quit"))
			{
				Hide();
			}
		}
	}
}
