using Mothership.GUI.Party;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal class AvatarViewLocalPlayer : MonoBehaviour, IChainListener, IInitialize
{
	public UITexture Texture;

	[SerializeField]
	private UIWidget GreenTick;

	[SerializeField]
	private UILabel TierLabel;

	[SerializeField]
	private UISprite TierBlueOverlay;

	[SerializeField]
	private UISprite GreyBackground;

	[SerializeField]
	private UISprite TierGreenReadyBackground;

	[SerializeField]
	private GameObject LeaderBlueOverlay;

	[SerializeField]
	private GameObject LeaderGreenOverlay;

	[SerializeField]
	private GameObject LeaderGreyOverlay;

	[Inject]
	internal AvatarPresenterLocalPlayer avatarPresenterLocalPlayer
	{
		private get;
		set;
	}

	public AvatarViewLocalPlayer()
		: this()
	{
	}

	public void UpdateBackgroundColour(PartyIconInfo info, bool isInParty)
	{
		if (info.isReadyState)
		{
			GreenTick.set_enabled(true);
		}
		else
		{
			GreenTick.set_enabled(false);
		}
		if (info.isLeader)
		{
			if (info.isReadyState)
			{
				SetLeaderLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Green);
			}
			else
			{
				SetLeaderLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Blue);
			}
		}
		else
		{
			SetLeaderLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Transparent);
		}
		if (!info.isLeader && !info.robotTierMatchesLeaderTier)
		{
			if (!isInParty)
			{
				SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Blue);
			}
			else
			{
				SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Grey);
			}
		}
		else if (info.isReadyState)
		{
			SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Green);
		}
		else
		{
			SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Blue);
		}
	}

	void IChainListener.Listen(object message)
	{
		avatarPresenterLocalPlayer.ReceiveMessage(message);
	}

	public void SetPlayerRobotTier(string tierString)
	{
		TierLabel.set_text(tierString);
	}

	private void Start()
	{
		avatarPresenterLocalPlayer.RegisterView(this);
	}

	private void SetLeaderLabelBackgroundColourState(PartyButtonBackgroundColourStatus status)
	{
		switch (status)
		{
		case PartyButtonBackgroundColourStatus.Transparent:
			LeaderBlueOverlay.SetActive(false);
			LeaderGreenOverlay.SetActive(false);
			LeaderGreyOverlay.SetActive(false);
			break;
		case PartyButtonBackgroundColourStatus.Blue:
			LeaderBlueOverlay.SetActive(true);
			LeaderGreenOverlay.SetActive(false);
			LeaderGreyOverlay.SetActive(false);
			break;
		case PartyButtonBackgroundColourStatus.Grey:
			LeaderBlueOverlay.SetActive(false);
			LeaderGreenOverlay.SetActive(false);
			LeaderGreyOverlay.SetActive(true);
			break;
		case PartyButtonBackgroundColourStatus.Green:
			LeaderBlueOverlay.SetActive(false);
			LeaderGreenOverlay.SetActive(true);
			LeaderGreyOverlay.SetActive(false);
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

	public void OnDependenciesInjected()
	{
		SetLeaderLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Transparent);
		SetTierLabelBackgroundColourState(PartyButtonBackgroundColourStatus.Blue);
		GreenTick.set_enabled(false);
	}
}
