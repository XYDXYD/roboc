using Services.Analytics;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Mothership.ItemShop
{
	internal class ItemShopBundleLayoutEngine : MultiEntityViewsEngine<ItemShopDisplayEntityView, ItemShopBundleEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly ItemShopDataSource _dataSource;

		private readonly LoadingIconPresenter _loadingIconPresenter;

		private readonly IAnalyticsRequestFactory _analyticsRequestFactory;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public ItemShopBundleLayoutEngine(ItemShopDataSource dataSource, LoadingIconPresenter loadingIconPresenter, IAnalyticsRequestFactory analyticsRequestFactory)
		{
			_dataSource = dataSource;
			_loadingIconPresenter = loadingIconPresenter;
			_analyticsRequestFactory = analyticsRequestFactory;
		}

		public void Ready()
		{
		}

		protected override void Add(ItemShopDisplayEntityView displayEntityView)
		{
			displayEntityView.itemShopDisplayComponent.refresh.NotifyOnValueSet((Action<int, bool>)RefreshData);
		}

		protected override void Remove(ItemShopDisplayEntityView displayEntityView)
		{
			displayEntityView.itemShopDisplayComponent.refresh.StopNotify((Action<int, bool>)RefreshData);
		}

		protected override void Add(ItemShopBundleEntityView productEntityView)
		{
			if (productEntityView.bundleComponent.bundle != null)
			{
				DisplayBundle(productEntityView);
			}
		}

		protected override void Remove(ItemShopBundleEntityView productEntityView)
		{
		}

		private void DisplayBundle(ItemShopBundleEntityView entityView)
		{
			ItemShopBundle bundle = entityView.bundleComponent.bundle;
			IItemShopBundleGuiComponent guiComponent = entityView.guiComponent;
			guiComponent.nameText = StringTableBase<StringTable>.Instance.GetString(bundle.BundleNameStrKey);
			guiComponent.categoryText = GetCategoryString(bundle.Category);
			guiComponent.currencyType = bundle.CurrencyType;
			guiComponent.costText = FormatPrice(bundle.Price);
			guiComponent.isFullSizeSprite = bundle.IsSpriteFullSize;
			guiComponent.spriteName = bundle.SpriteName;
			guiComponent.locked = !bundle.OwnsRequiredCube;
			guiComponent.discounted = bundle.Discounted;
			guiComponent.limitedEdition = bundle.LimitedEdition;
			if (bundle.Discounted)
			{
				int discountPercent = bundle.GetDiscountPercent();
				guiComponent.discountPercentText = StringTableBase<StringTable>.Instance.GetReplaceString("strPercentOff", "{percent}", discountPercent.ToString());
				guiComponent.nonDiscountedCostText = FormatPrice(bundle.Price);
				guiComponent.costText = FormatPrice(bundle.DiscountPrice);
			}
		}

		private static string FormatPrice(int price)
		{
			return price.ToString("N0");
		}

		private static string GetCategoryString(ItemShopCategory itemShopCategory)
		{
			switch (itemShopCategory)
			{
			case ItemShopCategory.GarageBaySkin:
				return StringTableBase<StringTable>.Instance.GetString("strItemShopTypeBaySkins");
			case ItemShopCategory.Bundle:
				return StringTableBase<StringTable>.Instance.GetString("strItemShopTypeBundle");
			default:
				return StringTableBase<StringTable>.Instance.GetString("strCosmetic");
			}
		}

		private void RefreshData(int entityID, bool refresh)
		{
			if (refresh)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshItemShop);
			}
		}

		private IEnumerator RefreshItemShop()
		{
			_loadingIconPresenter.NotifyLoading("LoadingItemShop");
			IEnumerator enumer = _dataSource.LoadData(clearCache: true);
			yield return enumer;
			ItemShopResponseData response = (ItemShopResponseData)enumer.Current;
			List<ItemShopBundle> bundles = response.ItemShopBundles;
			HandlePurchaseRestockAnalytics(bundles);
			Dictionary<ItemShopRecurrence, FasterList<ItemShopBundle>> result = new Dictionary<ItemShopRecurrence, FasterList<ItemShopBundle>>();
			int weeklyCount = 0;
			int dailyCount = 0;
			for (int i = 0; i < bundles.Count; i++)
			{
				ItemShopBundle itemShopBundle = bundles[i];
				ItemShopRecurrence recurrence = itemShopBundle.Recurrence;
				switch (recurrence)
				{
				case ItemShopRecurrence.Daily:
					dailyCount++;
					break;
				case ItemShopRecurrence.Weekly:
					weeklyCount++;
					break;
				}
				if (!result.ContainsKey(recurrence))
				{
					result.Add(recurrence, new FasterList<ItemShopBundle>());
				}
				result[recurrence].Add(itemShopBundle);
			}
			int bundleEntityViewCount = default(int);
			ItemShopBundleEntityView[] bundleEntityViews = entityViewsDB.QueryEntityViewsAsArray<ItemShopBundleEntityView>(ref bundleEntityViewCount);
			for (int j = 0; j < bundleEntityViewCount; j++)
			{
				ItemShopBundleEntityView itemShopBundleEntityView = bundleEntityViews[j];
				itemShopBundleEntityView.showComponent.isShown.set_value(false);
			}
			foreach (KeyValuePair<ItemShopRecurrence, FasterList<ItemShopBundle>> item in result)
			{
				UpdateBundlesCategory(item.Key, item.Value);
			}
			UpdateEmptySlots(ItemShopRecurrence.Daily, dailyCount);
			UpdateEmptySlots(ItemShopRecurrence.Weekly, weeklyCount);
			FasterListEnumerator<ItemShopDisplayEntityView> enumerator2 = entityViewsDB.QueryEntityViews<ItemShopDisplayEntityView>().GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					ItemShopDisplayEntityView current2 = enumerator2.get_Current();
					current2.itemShopDisplayComponent.dailyStockHash.set_value(response.dailyStockHash);
					current2.itemShopDisplayComponent.featuredStockHash.set_value(response.featuredStockHash);
					current2.itemShopDisplayComponent.refresh.set_value(false);
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			_loadingIconPresenter.NotifyLoadingDone("LoadingItemShop");
		}

		private void HandlePurchaseRestockAnalytics(List<ItemShopBundle> updatedBundles)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<ItemShopDisplayEntityView> enumerator = entityViewsDB.QueryEntityViews<ItemShopDisplayEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ItemShopDisplayEntityView current = enumerator.get_Current();
					RefreshReason lastRefreshReason = current.itemShopDisplayComponent.lastRefreshReason;
					if (lastRefreshReason == RefreshReason.ItemBought)
					{
						FasterReadOnlyList<ItemShopBundleEntityView> val = entityViewsDB.QueryEntityViews<ItemShopBundleEntityView>();
						foreach (ItemShopBundle updatedBundle in updatedBundles)
						{
							bool flag = true;
							FasterListEnumerator<ItemShopBundleEntityView> enumerator3 = val.GetEnumerator();
							try
							{
								while (enumerator3.MoveNext())
								{
									ItemShopBundleEntityView current3 = enumerator3.get_Current();
									ItemShopBundle bundle = current3.bundleComponent.bundle;
									if (bundle != null && bundle.BundleNameStrKey == updatedBundle.BundleNameStrKey)
									{
										flag = false;
										break;
									}
								}
							}
							finally
							{
								((IDisposable)enumerator3).Dispose();
							}
							if (flag)
							{
								TaskRunner.get_Instance().Run(ItemShopAnalyticsUtility.LogRestockedBundle(updatedBundle, lastRefreshReason, _analyticsRequestFactory));
							}
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private void UpdateBundlesCategory(ItemShopRecurrence recurrence, FasterList<ItemShopBundle> bundles)
		{
			int num = default(int);
			ItemShopBundleEntityView[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<ItemShopBundleEntityView>((int)recurrence, ref num);
			int i = 0;
			int j = 0;
			for (; i < bundles.get_Count(); i++)
			{
				ItemShopBundle itemShopBundle = bundles.get_Item(i);
				if (itemShopBundle.Recurrence == recurrence)
				{
					if (j < num)
					{
						ItemShopBundleEntityView itemShopBundleEntityView = array[j];
						itemShopBundleEntityView.bundleComponent.bundle = itemShopBundle;
						array[i].showComponent.isShown.set_value(true);
						DisplayBundle(itemShopBundleEntityView);
						j++;
					}
					else
					{
						Console.LogError("Received more bundles than expected");
					}
				}
			}
			for (; j < num; j++)
			{
				array[j].bundleComponent.bundle = null;
				array[j].showComponent.isShown.set_value(false);
			}
		}

		private void UpdateEmptySlots(ItemShopRecurrence recurrence, int nonEmptySlots)
		{
			int num = default(int);
			ItemShopEmptySlotEntityView[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<ItemShopEmptySlotEntityView>((int)recurrence, ref num);
			for (int i = 0; i < num; i++)
			{
				array[i].showComponent.isShown.set_value(i >= nonEmptySlots);
			}
		}
	}
}
