using Authentication;
using Fabric;
using Robocraft.GUI.Iteration2;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Party
{
	public class PartyIconView : MonoBehaviour, IChainListener, ISharedPartyIcon
	{
		[SerializeField]
		private int PositionIndex;

		[SerializeField]
		private UIWidget ButtonContainer;

		[SerializeField]
		private UITexture PlayerAvatar;

		[SerializeField]
		private UIWidget PlusButton;

		[SerializeField]
		private UIWidget PendingIcon;

		[SerializeField]
		private UIWidget ReadyIcon;

		[SerializeField]
		private UIWidget MemberInGame;

		[SerializeField]
		private UILabel TierLabel;

		[SerializeField]
		private UISprite TierBlueOverlay;

		[SerializeField]
		private UISprite GreyBackground;

		[SerializeField]
		private UISprite TierGreenReadyBackground;

		[SerializeField]
		private UIWidget LeaderBlueOverlay;

		[SerializeField]
		private UIWidget LeaderGreenOverlay;

		[SerializeField]
		private UIWidget LeaderGreyOverlay;

		private GenericTooltipArea tooltip;

		private PartyButtonInteraction buttonStateAdjuster;

		[SerializeField]
		private bool NormalPartyIcon;

		private PartyIconState _iconState;

		private BubbleSignal<IChainRoot> _bubble;

		[Inject]
		internal IPartyIconController partyIconController
		{
			private get;
			set;
		}

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		public bool IsLeader => partyIconController.IsLeader();

		public PartyIconView()
			: this()
		{
		}

		public bool CanDrag()
		{
			if (IsBlockedFromInteraction())
			{
				return false;
			}
			if (SlotIsAvailable())
			{
				return false;
			}
			string playerAssignedToSlot = partyIconController.PlayerAssignedToSlot;
			if (playerAssignedToSlot == User.Username && !partyIconController.IsLeader())
			{
				return false;
			}
			return true;
		}

		public int GetPositionIndex()
		{
			return PositionIndex;
		}

		public bool IsBlockedFromInteraction()
		{
			return partyIconController.IsBlockedFromInteraction;
		}

		public bool SlotIsAvailable()
		{
			if (_iconState == PartyIconState.AddMemberState || _iconState == PartyIconState.MemberPendingAccept)
			{
				return true;
			}
			return false;
		}

		void IChainListener.Listen(object message)
		{
			partyIconController.ReceiveMessage(message);
		}

		public void Start()
		{
			if (NormalPartyIcon)
			{
				Activate(null);
			}
		}

		public void Activate(int? indexOverride)
		{
			tooltip = this.get_gameObject().GetComponent<GenericTooltipArea>();
			buttonStateAdjuster = this.get_gameObject().GetComponent<PartyButtonInteraction>();
			buttonStateAdjuster.Activate();
			GenericWidgetFactory.BuildTooltipAreaExisting(this.get_gameObject(), gameObjectPool);
			if (indexOverride.HasValue)
			{
				PositionIndex = indexOverride.Value;
			}
			partyIconController.RegisterView(this);
			ChangeIconStatus(PartyIconState.AddMemberState);
			HideTierStatus();
			HideAllIcons();
			_bubble = new BubbleSignal<IChainRoot>(this.get_transform());
		}

		public void Disable()
		{
			buttonStateAdjuster.DisableButton();
		}

		public void Enable()
		{
			buttonStateAdjuster.EnableButton();
		}

		public void OnClick()
		{
			if (!IsBlockedFromInteraction())
			{
				MouseButton mouseButton;
				switch (UICamera.currentTouchID)
				{
				case -1:
					mouseButton = MouseButton.LEFT;
					break;
				case -2:
					mouseButton = MouseButton.RIGHT;
					break;
				default:
					mouseButton = MouseButton.MIDDLE;
					break;
				}
				Dispatch(new PartyMemberButtonClickMessage(_iconState, ButtonContainer, PositionIndex, mouseButton, partyIconController.PlayerAssignedToSlot));
			}
		}

		private void Dispatch(object message)
		{
			_bubble.TargetedDispatch<object>(message);
		}

		public Texture FetchCurrentAvatarTexture()
		{
			if (PlayerAvatar != null)
			{
				return PlayerAvatar.get_mainTexture();
			}
			return null;
		}

		public void AssignAvatarTexture(Texture newTexture)
		{
			if (PlayerAvatar != null)
			{
				PlayerAvatar.set_mainTexture(newTexture);
				PlayerAvatar.get_gameObject().SetActive(true);
			}
		}

		private void HideAllIcons()
		{
			PendingIcon.get_gameObject().SetActive(false);
			ReadyIcon.get_gameObject().SetActive(false);
			PlusButton.get_gameObject().SetActive(false);
			PlayerAvatar.get_gameObject().SetActive(false);
			MemberInGame.get_gameObject().SetActive(false);
			TierBlueOverlay.get_gameObject().SetActive(false);
			GreyBackground.get_gameObject().SetActive(false);
			TierGreenReadyBackground.get_gameObject().SetActive(false);
			LeaderBlueOverlay.get_gameObject().SetActive(false);
			LeaderGreenOverlay.get_gameObject().SetActive(false);
			LeaderGreyOverlay.get_gameObject().SetActive(false);
		}

		public void ChangeIconStatus(PartyIconState newState)
		{
			HideAllIcons();
			switch (newState)
			{
			case PartyIconState.AddMemberState:
				HideAllIcons();
				GreyBackground.get_gameObject().SetActive(false);
				HideTierStatus();
				PlusButton.get_gameObject().SetActive(true);
				if (_iconState == PartyIconState.MemberPendingAccept && !NormalPartyIcon)
				{
					EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[140], 0);
				}
				if (_iconState == PartyIconState.MemberShowAvatar && !NormalPartyIcon)
				{
					EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[140], 0);
				}
				break;
			case PartyIconState.MemberPendingAccept:
				HideAllIcons();
				GreyBackground.get_gameObject().SetActive(true);
				PlayerAvatar.get_gameObject().SetActive(true);
				PendingIcon.get_gameObject().SetActive(true);
				break;
			case PartyIconState.MemberShowAvatar:
				if (_iconState == PartyIconState.MemberPendingAccept && !NormalPartyIcon)
				{
					EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[139], 0);
				}
				HideAllIcons();
				GreyBackground.get_gameObject().SetActive(true);
				PlayerAvatar.get_gameObject().SetActive(true);
				break;
			case PartyIconState.MemberWaitingInQueue:
				HideAllIcons();
				GreyBackground.get_gameObject().SetActive(false);
				ReadyIcon.get_gameObject().SetActive(true);
				PlayerAvatar.get_gameObject().SetActive(true);
				break;
			case PartyIconState.MemberInGame:
				HideAllIcons();
				MemberInGame.get_gameObject().SetActive(true);
				PlayerAvatar.get_gameObject().SetActive(true);
				break;
			}
			_iconState = newState;
		}

		public void SetPlayerName(string name)
		{
			tooltip.text = name;
		}

		public void HideTierStatus()
		{
			TierLabel.set_text(string.Empty);
		}

		public void UpdateBackgroundColour(PartyIconInfo info)
		{
			if (info.emptySlot)
			{
				SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Transparent);
				SetLeaderLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Transparent);
			}
			else if (info.isLeader)
			{
				if (info.isReadyState)
				{
					SetLeaderLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Green);
					SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Green);
				}
				else if (!info.robotTierMatchesLeaderTier)
				{
					SetLeaderLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Grey);
					SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Grey);
				}
				else
				{
					SetLeaderLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Blue);
					SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Blue);
				}
			}
			else
			{
				SetLeaderLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Transparent);
				if (info.isReadyState)
				{
					SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Green);
				}
				else if (info.robotTierMatchesLeaderTier)
				{
					SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Blue);
				}
				else
				{
					SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Grey);
				}
			}
		}

		private void SetLeaderLabelBackgroundColourState(PartyButtonBackgroundColourStatus status)
		{
			switch (status)
			{
			case PartyButtonBackgroundColourStatus.Transparent:
				LeaderBlueOverlay.set_enabled(false);
				LeaderGreenOverlay.set_enabled(false);
				LeaderGreyOverlay.set_enabled(false);
				LeaderBlueOverlay.get_gameObject().SetActive(false);
				LeaderGreenOverlay.get_gameObject().SetActive(false);
				LeaderGreyOverlay.get_gameObject().SetActive(false);
				break;
			case PartyButtonBackgroundColourStatus.Blue:
				LeaderBlueOverlay.set_enabled(true);
				LeaderGreenOverlay.set_enabled(false);
				LeaderGreyOverlay.set_enabled(false);
				LeaderBlueOverlay.get_gameObject().SetActive(true);
				LeaderGreenOverlay.get_gameObject().SetActive(false);
				LeaderGreyOverlay.get_gameObject().SetActive(false);
				break;
			case PartyButtonBackgroundColourStatus.Grey:
				LeaderBlueOverlay.set_enabled(false);
				LeaderGreenOverlay.set_enabled(false);
				LeaderGreyOverlay.set_enabled(true);
				LeaderBlueOverlay.get_gameObject().SetActive(false);
				LeaderGreenOverlay.get_gameObject().SetActive(false);
				LeaderGreyOverlay.get_gameObject().SetActive(true);
				break;
			case PartyButtonBackgroundColourStatus.Green:
				LeaderBlueOverlay.set_enabled(false);
				LeaderGreenOverlay.set_enabled(true);
				LeaderGreyOverlay.set_enabled(false);
				LeaderBlueOverlay.get_gameObject().SetActive(false);
				LeaderGreenOverlay.get_gameObject().SetActive(true);
				LeaderGreyOverlay.get_gameObject().SetActive(false);
				break;
			}
		}

		private void SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus status)
		{
			switch (status)
			{
			case PartyButtonBackgroundColourStatus.Transparent:
				TierBlueOverlay.set_enabled(false);
				GreyBackground.set_enabled(false);
				TierGreenReadyBackground.set_enabled(false);
				break;
			case PartyButtonBackgroundColourStatus.Blue:
				TierBlueOverlay.set_enabled(true);
				TierBlueOverlay.get_gameObject().SetActive(true);
				GreyBackground.set_enabled(false);
				TierGreenReadyBackground.set_enabled(false);
				break;
			case PartyButtonBackgroundColourStatus.Grey:
				TierBlueOverlay.set_enabled(false);
				GreyBackground.set_enabled(true);
				GreyBackground.get_gameObject().SetActive(true);
				TierGreenReadyBackground.set_enabled(false);
				break;
			case PartyButtonBackgroundColourStatus.Green:
				TierBlueOverlay.set_enabled(false);
				GreyBackground.set_enabled(false);
				TierGreenReadyBackground.set_enabled(true);
				TierGreenReadyBackground.get_gameObject().SetActive(true);
				break;
			}
		}

		public void SetPlayerRobotTier(string tierString)
		{
			TierLabel.set_text(tierString);
		}
	}
}
