using Services.Analytics;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership.ItemShop
{
	internal class ItemShopCTAEngine : SingleEntityViewEngine<ItemShopDisplayEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private const string ITEM_SHOP_DAILY_HASH_FIELD = "ItemShopStockDailyHash";

		private const string ITEM_SHOP_FEATURED_HASH_FIELD = "ItemShopStockFeaturedHash";

		private const string ITEM_SHOP_SEEN_DAILY_HASH_FIELD = "ItemShopStockDailyHash_Seen";

		private const string ITEM_SHOP_SEEN_FEATURED_HASH_FIELD = "ItemShopStockFeaturedHash_Seen";

		private IAnalyticsRequestFactory _analyticsRequestFactory;

		private ItemShopDataSource _dataSource;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public ItemShopCTAEngine(ItemShopDataSource dataSource, IAnalyticsRequestFactory analyticsRequestFactory)
		{
			_analyticsRequestFactory = analyticsRequestFactory;
			_dataSource = dataSource;
		}

		public void Ready()
		{
		}

		protected override void Add(ItemShopDisplayEntityView itemShopDisplay)
		{
			itemShopDisplay.showComponent.isShown.NotifyOnValueSet((Action<int, bool>)HideCTAWhenItemShopIsShown);
			itemShopDisplay.itemShopDisplayComponent.dailyStockHash.NotifyOnValueSet((Action<int, int>)DetectDailyStockChanged);
			itemShopDisplay.itemShopDisplayComponent.featuredStockHash.NotifyOnValueSet((Action<int, int>)DetectFeaturedStockChanged);
		}

		protected override void Remove(ItemShopDisplayEntityView itemShopDisplay)
		{
			itemShopDisplay.showComponent.isShown.StopNotify((Action<int, bool>)HideCTAWhenItemShopIsShown);
			itemShopDisplay.itemShopDisplayComponent.dailyStockHash.StopNotify((Action<int, int>)DetectDailyStockChanged);
			itemShopDisplay.itemShopDisplayComponent.featuredStockHash.StopNotify((Action<int, int>)DetectFeaturedStockChanged);
		}

		private void HideCTAWhenItemShopIsShown(int entityId, bool shown)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			if (shown)
			{
				FasterReadOnlyList<ItemShopCTAEntityView> val = entityViewsDB.QueryEntityViews<ItemShopCTAEntityView>();
				bool flag = false;
				FasterListEnumerator<ItemShopCTAEntityView> enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ItemShopCTAEntityView current = enumerator.get_Current();
						flag |= current.showComponent.isShown.get_value();
						current.showComponent.isShown.set_value(false);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				if (flag)
				{
					PlayerPrefs.SetInt("ItemShopStockDailyHash_Seen", PlayerPrefs.GetInt("ItemShopStockDailyHash"));
					PlayerPrefs.SetInt("ItemShopStockFeaturedHash_Seen", PlayerPrefs.GetInt("ItemShopStockFeaturedHash"));
					TaskRunner.get_Instance().Run(ItemShopAnalyticsUtility.LogVisitedShop(_analyticsRequestFactory, val.get_Item(0).reasonComponent.dailyRestock, val.get_Item(0).reasonComponent.featuredRestock));
					FasterListEnumerator<ItemShopCTAEntityView> enumerator2 = val.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							ItemShopCTAEntityView current2 = enumerator2.get_Current();
							current2.reasonComponent.dailyRestock = false;
							current2.reasonComponent.featuredRestock = false;
						}
					}
					finally
					{
						((IDisposable)enumerator2).Dispose();
					}
				}
			}
		}

		private void DetectDailyStockChanged(int entityId, int dailyStockHash)
		{
			TaskRunner.get_Instance().Run(DetectStockChanged(entityId, dailyStockHash, "ItemShopStockDailyHash", "ItemShopStockDailyHash_Seen", ItemShopRecurrence.Daily));
		}

		private void DetectFeaturedStockChanged(int entityId, int featuredStockHash)
		{
			TaskRunner.get_Instance().Run(DetectStockChanged(entityId, featuredStockHash, "ItemShopStockFeaturedHash", "ItemShopStockFeaturedHash_Seen", ItemShopRecurrence.Weekly));
		}

		private IEnumerator DetectStockChanged(int entityId, int hash, string detectField, string seenField, ItemShopRecurrence section)
		{
			if (PlayerPrefs.HasKey(detectField) && PlayerPrefs.GetInt(detectField) == hash)
			{
				yield break;
			}
			IEnumerator enumer = _dataSource.LoadData();
			yield return enumer;
			ItemShopResponseData response = (ItemShopResponseData)enumer.Current;
			List<ItemShopBundle> bundles = response.ItemShopBundles;
			PlayerPrefs.SetInt(detectField, hash);
			TaskRunner.get_Instance().Run(LogRestockAnalytics(RefreshReason.ShopRefresh, section, bundles));
			if (!PlayerPrefs.HasKey(seenField) || PlayerPrefs.GetInt(seenField) != hash)
			{
				if (IsItemShopOpen())
				{
					PlayerPrefs.SetInt(seenField, hash);
				}
				else if (bundles.Count > 0)
				{
					FasterListEnumerator<ItemShopCTAEntityView> enumerator = entityViewsDB.QueryEntityViews<ItemShopCTAEntityView>().GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							ItemShopCTAEntityView current = enumerator.get_Current();
							current.showComponent.isShown.set_value(true);
							current.reasonComponent.dailyRestock |= (section == ItemShopRecurrence.Daily);
							current.reasonComponent.featuredRestock |= (section == ItemShopRecurrence.Weekly);
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				else
				{
					PlayerPrefs.SetInt(seenField, hash);
				}
			}
		}

		private bool IsItemShopOpen()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<ItemShopDisplayEntityView> enumerator = entityViewsDB.QueryEntityViews<ItemShopDisplayEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ItemShopDisplayEntityView current = enumerator.get_Current();
					if (current.showComponent.isShown.get_value())
					{
						return true;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return false;
		}

		private IEnumerator LogRestockAnalytics(RefreshReason reason, ItemShopRecurrence section, List<ItemShopBundle> bundles)
		{
			foreach (ItemShopBundle bundle in bundles)
			{
				if (bundle.Recurrence == section)
				{
					yield return ItemShopAnalyticsUtility.LogRestockedBundle(bundle, reason, _analyticsRequestFactory);
				}
			}
		}
	}
}
