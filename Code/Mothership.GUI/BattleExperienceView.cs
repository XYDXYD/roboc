using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Fabric;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership.GUI
{
	internal class BattleExperienceView : MonoBehaviour, IChainRoot, IChainListener, IInitialize
	{
		public Color disabledRowColor;

		public GameObject battleExperienceLabel;

		public GameObject partyBonusCaptionLabel;

		public GameObject partyBonusLabel;

		public GameObject masteryBonusCaptionLabel;

		public GameObject masteryBonusLabel;

		public GameObject premiumExperienceCaptionLabel;

		public GameObject premiumExperienceLabel;

		public GameObject tierBonusCaptionLabel;

		public GameObject tierBonusLabel;

		[SerializeField]
		private GameObject tierBonusRoot;

		public GameObject totalLabel;

		public GameObject seasonExperienceLabel;

		public GameObject clanAverageExperienceLabel;

		public GameObject clanTotalExperienceLabel;

		public GameObject clanRobitsLabel;

		public GameObject playerRobitsLabel;

		public GameObject playerPremiumRobitsCaptionLabel;

		public GameObject playerPremiumRobitsLabel;

		public GameObject longPlayReductionCaptionLabel;

		public GameObject longPlayReductionLabel;

		public GameObject longPlayWarningMessage50Percent;

		public GameObject longPlayWarningMessage100Percent;

		public GameObject[] shownWithClan;

		public GameObject[] shownWithoutClan;

		[SerializeField]
		private AnimationClip entryAnimation;

		[SerializeField]
		private AnimationClip exitAnimation;

		[SerializeField]
		private UIWidget[] breakdownAnimationItems;

		[SerializeField]
		private string[] breakdownAnimationSounds;

		[SerializeField]
		private float breakdownItemDelay = 0.5f;

		[SerializeField]
		private GameObject getPremiumButton;

		private Animation _animationComponent;

		private SignalChain _signal;

		private Sequence _breakdownSequence;

		[Inject]
		internal BattleExperiencePresenter presenter
		{
			get;
			set;
		}

		public BattleExperienceView()
			: this()
		{
		}

		private void Awake()
		{
			this.get_gameObject().SetActive(false);
			_animationComponent = this.GetComponent<Animation>();
		}

		internal void Show(bool hasPremium, bool canBuyPremium)
		{
			this.get_gameObject().SetActive(true);
			getPremiumButton.SetActive(!hasPremium && canBuyPremium);
		}

		internal void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
		}

		public void ShowLongPlayWarningElements(BattleExperienceDataSource.LongPlayWarningMessageType warningMessageType)
		{
			switch (warningMessageType)
			{
			case BattleExperienceDataSource.LongPlayWarningMessageType.Warning_50Percent:
				longPlayWarningMessage50Percent.SetActive(true);
				longPlayWarningMessage100Percent.SetActive(false);
				break;
			case BattleExperienceDataSource.LongPlayWarningMessageType.Warning_100Percent:
				longPlayWarningMessage50Percent.SetActive(false);
				longPlayWarningMessage100Percent.SetActive(true);
				break;
			}
			longPlayReductionCaptionLabel.SetActive(true);
			longPlayReductionLabel.SetActive(true);
		}

		public void HideLongPlayWarningElements()
		{
			longPlayReductionCaptionLabel.SetActive(false);
			longPlayReductionLabel.SetActive(false);
			longPlayWarningMessage50Percent.SetActive(false);
			longPlayWarningMessage100Percent.SetActive(false);
		}

		public void PlayShowAnimation()
		{
			HideBreakdownWithAlpha();
			if (entryAnimation != null)
			{
				_animationComponent.Play(entryAnimation.get_name());
			}
		}

		internal void ShownWithClan(bool hasClan)
		{
			for (int i = 0; i < shownWithClan.Length; i++)
			{
				shownWithClan[i].SetActive(hasClan);
			}
		}

		internal void ShownWithoutClan(bool hasClan)
		{
			for (int i = 0; i < shownWithoutClan.Length; i++)
			{
				shownWithoutClan[i].SetActive(!hasClan);
			}
		}

		public void PlayHideAnimation()
		{
			if (exitAnimation != null)
			{
				_animationComponent.Play(exitAnimation.get_name());
			}
		}

		public void Listen(object message)
		{
			if (message is ButtonType)
			{
				switch ((ButtonType)message)
				{
				case ButtonType.Close:
					presenter.OnAskClose();
					break;
				case ButtonType.PurchasePremium:
					presenter.PurchasePremiumPressed();
					break;
				}
			}
		}

		public void BroadcastDownMessage<T>(T message)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if (_signal == null)
			{
				_signal = new SignalChain(this.get_transform());
			}
			_signal.DeepBroadcast<T>(message);
		}

		internal bool IsAnimationPlaying()
		{
			return _animationComponent.get_isPlaying() || _breakdownSequence != null;
		}

		private void HideBreakdownWithAlpha()
		{
			for (int i = 0; i < breakdownAnimationItems.Length; i++)
			{
				UIWidget val = breakdownAnimationItems[i];
				val.set_alpha(0f);
			}
		}

		private unsafe void PlayRewardList()
		{
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Expected O, but got Unknown
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Expected O, but got Unknown
			Sequence val = DOTween.Sequence();
			for (int i = 0; i < breakdownAnimationItems.Length; i++)
			{
				UIWidget item = breakdownAnimationItems[i];
				if (item.get_gameObject().get_activeSelf())
				{
					string sound = breakdownAnimationSounds[i % breakdownAnimationSounds.Length];
					_003CPlayRewardList_003Ec__AnonStorey1 _003CPlayRewardList_003Ec__AnonStorey;
					TweenSettingsExtensions.Append(val, TweenSettingsExtensions.OnStart<TweenerCore<float, float, FloatOptions>>(DOTween.To(new DOGetter<float>((object)_003CPlayRewardList_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)_003CPlayRewardList_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), 1f, breakdownItemDelay), new TweenCallback((object)_003CPlayRewardList_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
				}
			}
			TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_breakdownSequence = val;
		}

		private void PlayBreakdownItemSound(string s)
		{
			EventManager.get_Instance().PostEvent(s, 0, (object)null, this.get_gameObject());
		}

		internal void SetTierBonusActive(bool active)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			tierBonusRoot.get_gameObject().SetActive(true);
			if (!active)
			{
				UIWidget[] componentsInChildren = tierBonusRoot.GetComponentsInChildren<UIWidget>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].set_color(disabledRowColor);
				}
			}
		}
	}
}
