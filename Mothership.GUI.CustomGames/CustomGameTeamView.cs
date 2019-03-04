using Mothership.GUI.Party;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership.GUI.CustomGames
{
	internal class CustomGameTeamView : MonoBehaviour, IChainListener, IChainRoot
	{
		[SerializeField]
		private CustomGameTeamChoice TeamChoice;

		[SerializeField]
		private UIWidget SizeReferenceFirstPlayerLargeIcon;

		[SerializeField]
		private UIWidget SizeReferenceRegularIcon;

		[SerializeField]
		private UIWidget InvitePopupReferenceRect;

		[SerializeField]
		private UIWidget TeamAnchorRect;

		[SerializeField]
		private GameObject DragIcon;

		private UIWidget _firstIcon;

		private CustomGameTeamController _controller;

		private SignalChain _signal;

		private BubbleSignal<CustomGamePartyGUIView> _bubble;

		public GameObject DragIconToDisplay => DragIcon;

		public CustomGameTeamChoice TeamRepresentation => TeamChoice;

		public CustomGameTeamView()
			: this()
		{
		}

		public void InjectController(CustomGameTeamController controller)
		{
			_controller = controller;
		}

		public void Initialise()
		{
			BuildSignalAndBubble();
		}

		public unsafe void OnEnable()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			UICamera.onScreenResize = Delegate.Combine((Delegate)UICamera.onScreenResize, (Delegate)new OnScreenResize((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public unsafe void OnDisable()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			UICamera.onScreenResize = Delegate.Remove((Delegate)UICamera.onScreenResize, (Delegate)new OnScreenResize((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Listen(object message)
		{
			_controller.HandleMessage(message);
		}

		public void ShowPartyInviteForCustomGame(UIWidget anchorWidget)
		{
			_bubble.TargetedDispatch<ShowPartyInviteDropDownMessageForCustomGame>(new ShowPartyInviteDropDownMessageForCustomGame(anchorWidget, null, TeamRepresentation));
		}

		public void ShowContextMenuForCustomGame(UIWidget anchorWidget, string memberName, PartyPopupMenuItems menuItemToShow)
		{
			ShowPartyPopupMenuMessage showPartyPopupMenuMessage = new ShowPartyPopupMenuMessage(anchorWidget, memberName, menuItemToShow);
			_bubble.TargetedDispatch<ShowPartyPopupMenuMessage>(showPartyPopupMenuMessage);
		}

		public void SetLargePlayerAvatar(bool setting)
		{
			if (TeamChoice == CustomGameTeamChoice.TeamA)
			{
				SetLargeAvatarForTeamA(setting, _firstIcon);
			}
			else
			{
				SetLargeAvatarForTeamB(setting, _firstIcon);
			}
		}

		private void SetLargeAvatarForTeamA(bool isLarge, UIWidget avatarIcon)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			if (isLarge)
			{
				AnchorPoint rightAnchor = avatarIcon.rightAnchor;
				Vector2 localSize = SizeReferenceFirstPlayerLargeIcon.get_localSize();
				rightAnchor.absolute = (int)localSize.x;
				AnchorPoint bottomAnchor = avatarIcon.bottomAnchor;
				Vector2 localSize2 = SizeReferenceFirstPlayerLargeIcon.get_localSize();
				bottomAnchor.absolute = (int)(0f - localSize2.y);
				InvitePopupReferenceRect.leftAnchor.relative = 1f;
			}
			else
			{
				AnchorPoint rightAnchor2 = avatarIcon.rightAnchor;
				Vector2 localSize3 = SizeReferenceRegularIcon.get_localSize();
				rightAnchor2.absolute = (int)localSize3.x;
				AnchorPoint bottomAnchor2 = avatarIcon.bottomAnchor;
				Vector2 localSize4 = SizeReferenceRegularIcon.get_localSize();
				bottomAnchor2.absolute = (int)(0f - localSize4.y);
				InvitePopupReferenceRect.leftAnchor.relative = 0f;
			}
		}

		private void SetLargeAvatarForTeamB(bool isLarge, UIWidget avatarIcon)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			if (isLarge)
			{
				AnchorPoint leftAnchor = avatarIcon.leftAnchor;
				Vector2 localSize = SizeReferenceFirstPlayerLargeIcon.get_localSize();
				leftAnchor.absolute = (int)(0f - localSize.x);
				AnchorPoint bottomAnchor = avatarIcon.bottomAnchor;
				Vector2 localSize2 = SizeReferenceFirstPlayerLargeIcon.get_localSize();
				bottomAnchor.absolute = (int)(0f - localSize2.y);
				InvitePopupReferenceRect.rightAnchor.relative = 0f;
			}
			else
			{
				AnchorPoint leftAnchor2 = avatarIcon.leftAnchor;
				Vector2 localSize3 = SizeReferenceRegularIcon.get_localSize();
				leftAnchor2.absolute = (int)(0f - localSize3.x);
				AnchorPoint bottomAnchor2 = avatarIcon.bottomAnchor;
				Vector2 localSize4 = SizeReferenceRegularIcon.get_localSize();
				bottomAnchor2.absolute = (int)(0f - localSize4.y);
				InvitePopupReferenceRect.rightAnchor.relative = 1f;
			}
		}

		public void BuildSignalAndBubble()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			_signal = new SignalChain(this.get_transform());
			_bubble = new BubbleSignal<CustomGamePartyGUIView>(this.get_transform());
		}

		public void DeepBroadcast<T>(T message)
		{
			_signal.DeepBroadcast<T>(message);
		}

		public void Broadcast(object message)
		{
			_signal.Broadcast<object>(message);
		}

		public PartyIconView[] GetExistingIconList()
		{
			return this.GetComponentsInChildren<PartyIconView>();
		}

		public void AnchorIconList(PartyIconView[] icons)
		{
			UIWidget previousButton = null;
			_firstIcon = null;
			foreach (PartyIconView partyIconView in icons)
			{
				UIWidget component = partyIconView.get_gameObject().GetComponent<UIWidget>();
				if (TeamChoice == CustomGameTeamChoice.TeamA)
				{
					AnchorIconOnLeftRelativeToPrevious(component, previousButton);
				}
				else
				{
					AnchorIconOnRightRelativeToPrevious(component, previousButton);
				}
				previousButton = component;
			}
		}

		private void ReAnchorIconList()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			PartyIconView[] componentsInChildren = this.GetComponentsInChildren<PartyIconView>();
			PartyIconView[] array = componentsInChildren;
			foreach (PartyIconView partyIconView in array)
			{
				UIWidget component = partyIconView.get_gameObject().GetComponent<UIWidget>();
				Vector2 localSize = SizeReferenceRegularIcon.get_localSize();
				if (partyIconView.IsLeader)
				{
					localSize = SizeReferenceFirstPlayerLargeIcon.get_localSize();
				}
				if (TeamChoice == CustomGameTeamChoice.TeamA)
				{
					if (_firstIcon == component)
					{
						component.rightAnchor.Set(0f, localSize.x);
						component.bottomAnchor.Set(1f, 0f - localSize.y);
					}
					else
					{
						component.rightAnchor.Set(1f, localSize.x + 2f);
						component.bottomAnchor.Set(1f, 0f - localSize.y);
					}
				}
				else if (_firstIcon == component)
				{
					AnchorPoint leftAnchor = component.leftAnchor;
					Vector2 localSize2 = SizeReferenceRegularIcon.get_localSize();
					leftAnchor.Set(1f, 0f - localSize2.x);
					AnchorPoint bottomAnchor = component.bottomAnchor;
					Vector2 localSize3 = SizeReferenceRegularIcon.get_localSize();
					bottomAnchor.Set(1f, 0f - localSize3.y);
				}
				else
				{
					AnchorPoint leftAnchor2 = component.leftAnchor;
					Vector2 localSize4 = SizeReferenceRegularIcon.get_localSize();
					leftAnchor2.Set(0f, 0f - (localSize4.x + 2f));
					AnchorPoint bottomAnchor2 = component.bottomAnchor;
					Vector2 localSize5 = SizeReferenceRegularIcon.get_localSize();
					bottomAnchor2.Set(1f, 0f - localSize5.y);
				}
			}
		}

		private void AnchorIconOnLeftRelativeToPrevious(UIWidget buttonWidget, UIWidget previousButton)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			buttonWidget.get_gameObject().get_transform().set_parent(TeamAnchorRect.get_gameObject().get_transform());
			buttonWidget.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			if (previousButton == null)
			{
				_firstIcon = buttonWidget;
				buttonWidget.SetAnchor(TeamAnchorRect.get_transform());
				buttonWidget.leftAnchor.Set(0f, 0f);
				AnchorPoint rightAnchor = buttonWidget.rightAnchor;
				Vector2 localSize = SizeReferenceRegularIcon.get_localSize();
				rightAnchor.Set(0f, localSize.x);
				buttonWidget.topAnchor.Set(1f, 0f);
				AnchorPoint bottomAnchor = buttonWidget.bottomAnchor;
				Vector2 localSize2 = SizeReferenceRegularIcon.get_localSize();
				bottomAnchor.Set(1f, 0f - localSize2.y);
			}
			else
			{
				buttonWidget.SetAnchor(previousButton.get_transform());
				buttonWidget.leftAnchor.Set(1f, 2f);
				AnchorPoint rightAnchor2 = buttonWidget.rightAnchor;
				Vector2 localSize3 = SizeReferenceRegularIcon.get_localSize();
				rightAnchor2.Set(1f, localSize3.x + 2f);
				buttonWidget.topAnchor.Set(1f, 0f);
				AnchorPoint bottomAnchor2 = buttonWidget.bottomAnchor;
				Vector2 localSize4 = SizeReferenceRegularIcon.get_localSize();
				bottomAnchor2.Set(1f, 0f - localSize4.y);
			}
		}

		private void AnchorIconOnRightRelativeToPrevious(UIWidget buttonWidget, UIWidget previousButton)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			buttonWidget.get_gameObject().get_transform().set_parent(TeamAnchorRect.get_gameObject().get_transform());
			if (previousButton == null)
			{
				_firstIcon = buttonWidget;
				buttonWidget.SetAnchor(TeamAnchorRect.get_transform());
				AnchorPoint leftAnchor = buttonWidget.leftAnchor;
				Vector2 localSize = SizeReferenceRegularIcon.get_localSize();
				leftAnchor.Set(1f, 0f - localSize.x);
				buttonWidget.rightAnchor.Set(1f, 0f);
				buttonWidget.topAnchor.Set(1f, 0f);
				AnchorPoint bottomAnchor = buttonWidget.bottomAnchor;
				Vector2 localSize2 = SizeReferenceRegularIcon.get_localSize();
				bottomAnchor.Set(1f, 0f - localSize2.y);
			}
			else
			{
				buttonWidget.SetAnchor(previousButton.get_transform());
				AnchorPoint leftAnchor2 = buttonWidget.leftAnchor;
				Vector2 localSize3 = SizeReferenceRegularIcon.get_localSize();
				leftAnchor2.Set(0f, 0f - (localSize3.x + 2f));
				buttonWidget.rightAnchor.Set(0f, -2f);
				buttonWidget.topAnchor.Set(1f, 0f);
				AnchorPoint bottomAnchor2 = buttonWidget.bottomAnchor;
				Vector2 localSize4 = SizeReferenceRegularIcon.get_localSize();
				bottomAnchor2.Set(1f, 0f - localSize4.y);
			}
		}
	}
}
