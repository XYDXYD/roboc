using Game.ECS.GUI.Components;
using Svelto.ECS;
using System;
using UnityEngine;

namespace Mothership.ItemShop
{
	internal class ItemShopDisplayImplementor : MonoBehaviour, IItemShopDisplayComponent, IShowComponent
	{
		[SerializeField]
		private UILabel featureTimeRemainingUILabel;

		[SerializeField]
		private UIWidget featuredUIWidget;

		[SerializeField]
		private GameObject featuredProductTemplate;

		[SerializeField]
		private GameObject _featuredEmptySlotTemplate;

		[SerializeField]
		private int _maxShownFeaturedBundles = 3;

		[SerializeField]
		private UILabel dailyTimeRemainingUILabel;

		[SerializeField]
		private UIWidget dailyUIWidget;

		[SerializeField]
		private GameObject dailyProductTemplate;

		[SerializeField]
		private GameObject _dailyEmptySlotTemplate;

		[SerializeField]
		private int _maxShownDailyBundles = 6;

		public GameObject popup;

		GameObject IItemShopDisplayComponent.featuredProductTemplate
		{
			get
			{
				return featuredProductTemplate;
			}
		}

		GameObject IItemShopDisplayComponent.dailyProductTemplate
		{
			get
			{
				return dailyProductTemplate;
			}
		}

		public UIWidget featuredUiWidget => featuredUIWidget;

		public UIWidget dailyUiWidget => dailyUIWidget;

		public DispatchOnChange<bool> refresh
		{
			get;
			private set;
		}

		public RefreshReason lastRefreshReason
		{
			get;
			set;
		}

		public DispatchOnChange<bool> isShown
		{
			get;
			private set;
		}

		public string remainingFeaturedTime
		{
			set
			{
				featureTimeRemainingUILabel.set_text(value);
			}
		}

		public string remainingDailyTime
		{
			set
			{
				dailyTimeRemainingUILabel.set_text(value);
			}
		}

		public DispatchOnChange<int> dailyStockHash
		{
			get;
			private set;
		}

		public DispatchOnChange<int> featuredStockHash
		{
			get;
			private set;
		}

		public GameObject featuredEmptySlotTemplate => _featuredEmptySlotTemplate;

		public GameObject dailyEmptySlotTemplate => _dailyEmptySlotTemplate;

		public int maxShownFeaturedBundles => _maxShownFeaturedBundles;

		public int maxShownDailyBundles => _maxShownDailyBundles;

		public ItemShopDisplayImplementor()
			: this()
		{
		}

		public void Initialize(int entityId)
		{
			refresh = new DispatchOnChange<bool>(entityId);
			isShown = new DispatchOnChange<bool>(entityId);
			dailyStockHash = new DispatchOnChange<int>(entityId);
			featuredStockHash = new DispatchOnChange<int>(entityId);
			isShown.NotifyOnValueSet((Action<int, bool>)Show);
		}

		private void Show(int entityId, bool show)
		{
			this.get_gameObject().SetActive(show);
		}
	}
}
