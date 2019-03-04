using Game.RoboPass.GUI.Components;
using Svelto.ECS;
using System;
using UnityEngine;

namespace Game.RoboPass.GUI.Implementors
{
	public class BattleSummaryRewardItemPurchaseImplementor : MonoBehaviour, IBattleSummaryRewardItemPurchaseComponent
	{
		[SerializeField]
		private UIButton _summaryPanelBuyButton;

		[SerializeField]
		private UIButton _rewardsPanelBuyButton;

		private DispatchOnSet<bool> _summaryPanelBuyNowClicked;

		private DispatchOnSet<bool> _rewardsPanelBuyNowClicked;

		public DispatchOnSet<bool> SummaryPanelBuyNowClicked => _summaryPanelBuyNowClicked;

		public DispatchOnSet<bool> RewardsPanelBuyNowClicked => _rewardsPanelBuyNowClicked;

		public BattleSummaryRewardItemPurchaseImplementor()
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
			_summaryPanelBuyNowClicked = new DispatchOnSet<bool>(id);
			_summaryPanelBuyButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			_rewardsPanelBuyNowClicked = new DispatchOnSet<bool>(id);
			_rewardsPanelBuyButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		private void HandleSummaryPanelBuyNowClicked()
		{
			_summaryPanelBuyNowClicked.set_value(true);
		}

		private void HandleRewardsPanelBuyNowClicked()
		{
			_summaryPanelBuyNowClicked.set_value(true);
		}
	}
}
