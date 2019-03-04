using Game.RoboPass.GUI.Components;
using Svelto.ECS;
using System;
using System.Diagnostics;
using UnityEngine;

namespace Game.RoboPass.GUI.Implementors
{
	public class RewardedItemsPanelImplementor : MonoBehaviour, IBattleSummaryRewardedItemsPanelComponent
	{
		[SerializeField]
		private GameObject _deluxeRewardItem;

		[SerializeField]
		private GameObject _freeRewardItem;

		[SerializeField]
		private GameObject rewardReceivedButton;

		[SerializeField]
		private GameObject rewardReceivedText;

		[SerializeField]
		private UIButton _collectRewardsButton;

		[SerializeField]
		private UIButton continueButton;

		[SerializeField]
		private UIButton getRoboPassPlusButton;

		[SerializeField]
		private UIGrid _rewardedItemsGrid;

		[SerializeField]
		private UILabel descLabel;

		[SerializeField]
		private UILabel titleLabel;

		public UIGrid RewardsGrid => _rewardedItemsGrid;

		public GameObject FreeRewardItem => _freeRewardItem;

		public GameObject DeluxeRewardItem => _deluxeRewardItem;

		public bool FreeRewardItemActive
		{
			set
			{
				_freeRewardItem.SetActive(value);
			}
		}

		public bool DeluxeRewardItemActive
		{
			set
			{
				_deluxeRewardItem.SetActive(value);
			}
		}

		public bool PanelActive
		{
			set
			{
				this.get_gameObject().SetActive(value);
			}
		}

		public DispatchOnSet<bool> CollectRewardsClicked
		{
			get;
			private set;
		}

		public DispatchOnSet<bool> ContinueClicked
		{
			get;
			private set;
		}

		public DispatchOnSet<bool> GetRoboPassPlusClicked
		{
			get;
			private set;
		}

		public string TitleMsg
		{
			set
			{
				titleLabel.set_text(Localization.Get(value, true));
			}
		}

		public string DescMsg
		{
			set
			{
				descLabel.set_text(Localization.Get(value, true));
			}
		}

		public string TitleRewardUnlocked => "strRoboPassRewardUnlockedTitle";

		public string TitleRewardReceived => "strRoboPassRewardTitle";

		public string DescRewardUnlocked => "strRoboPassRewardUnlockedDesc";

		public string DescRewardReceived => "strRoboPassRewardDesc";

		public bool ButtonCollectActive
		{
			set
			{
				_collectRewardsButton.get_gameObject().SetActive(value);
			}
		}

		public bool ButtonContinueActive
		{
			set
			{
				continueButton.get_gameObject().SetActive(value);
			}
		}

		public bool ButtonGetRoboPassPlusActive
		{
			set
			{
				getRoboPassPlusButton.get_gameObject().SetActive(value);
			}
		}

		public bool RoboPassPlusRewardTextActive
		{
			set
			{
				rewardReceivedButton.get_gameObject().SetActive(value);
				rewardReceivedText.get_gameObject().SetActive(value);
			}
		}

		public RewardedItemsPanelImplementor()
			: this()
		{
		}

		internal unsafe void Initialize(int id)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected O, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected O, but got Unknown
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Expected O, but got Unknown
			CollectRewardsClicked = new DispatchOnSet<bool>(id);
			_collectRewardsButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			ContinueClicked = new DispatchOnSet<bool>(id);
			continueButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			GetRoboPassPlusClicked = new DispatchOnSet<bool>(id);
			getRoboPassPlusButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		private void OnCollectRewardsClick()
		{
			CollectRewardsClicked.set_value(true);
		}

		private void OnContinueClick()
		{
			ContinueClicked.set_value(true);
		}

		private void OnGetRoboPassPlusClick()
		{
			GetRoboPassPlusClicked.set_value(true);
		}

		[Conditional("DEBUG")]
		private void CheckExposedFields()
		{
		}
	}
}
