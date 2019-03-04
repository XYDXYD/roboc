using Svelto.ECS;
using UnityEngine;

namespace Mothership.ItemShop
{
	internal interface IItemShopDisplayComponent
	{
		UIWidget featuredUiWidget
		{
			get;
		}

		GameObject featuredProductTemplate
		{
			get;
		}

		GameObject featuredEmptySlotTemplate
		{
			get;
		}

		UIWidget dailyUiWidget
		{
			get;
		}

		GameObject dailyProductTemplate
		{
			get;
		}

		GameObject dailyEmptySlotTemplate
		{
			get;
		}

		string remainingFeaturedTime
		{
			set;
		}

		string remainingDailyTime
		{
			set;
		}

		DispatchOnChange<bool> refresh
		{
			get;
		}

		RefreshReason lastRefreshReason
		{
			get;
			set;
		}

		DispatchOnChange<int> dailyStockHash
		{
			get;
		}

		DispatchOnChange<int> featuredStockHash
		{
			get;
		}

		int maxShownFeaturedBundles
		{
			get;
		}

		int maxShownDailyBundles
		{
			get;
		}
	}
}
