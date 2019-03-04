using Robocraft.GUI;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;

namespace Mothership.GUI.CustomGames
{
	internal class CustomGamePopupMenuController : GenericPopupMenuController
	{
		private const string KICK_FROM_PARTY_ACTION = "KickFromParty";

		private const string CANCEL_PENDING_INVITE = "CancelPendingInvite";

		private const string LEAVE_PARTY = "LeaveParty";

		private string _clickedMember;

		private BubbleSignal<CustomGamePartyGUIView> _bubble;

		[Inject]
		internal IGUIInputController inputController
		{
			private get;
			set;
		}

		private void ShowPopupMenuForContext(UIWidget anchorPoint, PartyPopupMenuItems menuItems)
		{
			PositionUnAnchoredMenu(anchorPoint);
			ResetMenu();
			if (GenericPopupMenuController.isFlagSet(PartyPopupMenuItems.CancelPendingInvitation, menuItems))
			{
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strCancelCustomGamePartyInvite"), "CancelPendingInvite");
			}
			if (GenericPopupMenuController.isFlagSet(PartyPopupMenuItems.RemoveFromParty, menuItems))
			{
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strKickFromCustomGame"), "KickFromParty");
			}
			if (GenericPopupMenuController.isFlagSet(PartyPopupMenuItems.LeaveParty, menuItems))
			{
				AddItemToMenu(StringTableBase<StringTable>.Instance.GetString("strLeavePlatoon"), "LeaveParty");
			}
			Show();
		}

		internal override void SetView(GenericPopupMenuView view)
		{
			_bubble = new BubbleSignal<CustomGamePartyGUIView>(view.get_transform());
			base.SetView(view);
			Hide();
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
					if (originator == "LeaveParty")
					{
						Hide();
						_bubble.TargetedDispatch<PartyPopupMenuClickMessage>(new PartyPopupMenuClickMessage(_clickedMember, PartyPopupMenuItems.LeaveParty));
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
			if (message.GetType() == typeof(HidePartyPopupMenuMessage))
			{
				Hide();
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
