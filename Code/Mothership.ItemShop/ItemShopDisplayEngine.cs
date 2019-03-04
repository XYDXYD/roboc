using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;

namespace Mothership.ItemShop
{
	internal class ItemShopDisplayEngine : SingleEntityViewEngine<ItemShopDisplayEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private const int TOTAL_DAYS_TIME_REMAINING_STR_FORMAT = 1;

		private const int RESET_HOUR = 15;

		private DateTime _featuredExpiryDT;

		private DateTime _dailyExpiryDT;

		private ItemShopDataSource _dataSource;

		private ItemShopGUIFactory _factory;

		private LoadingIconPresenter _loadingIconPresenter;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public ItemShopDisplayEngine(ItemShopDataSource dataSource, ItemShopGUIFactory factory, LoadingIconPresenter loadingIconPresenter)
		{
			_dataSource = dataSource;
			_factory = factory;
			_loadingIconPresenter = loadingIconPresenter;
		}

		public void Ready()
		{
			_dailyExpiryDT = GenerateDailyBundleExpiryTime();
			_featuredExpiryDT = GenerateWeeklyBundleExpiryTime();
			TaskRunner.get_Instance().Run((Func<IEnumerator>)UpdateExpiryTimeLabels);
		}

		protected override void Add(ItemShopDisplayEntityView entityView)
		{
			entityView.showComponent.isShown.NotifyOnValueSet((Action<int, bool>)OnShow);
			TaskRunner.get_Instance().Run((Func<IEnumerator>)BuildGUI);
		}

		protected override void Remove(ItemShopDisplayEntityView entityView)
		{
			entityView.showComponent.isShown.StopNotify((Action<int, bool>)OnShow);
		}

		private void OnShow(int entityId, bool shown)
		{
			if (!shown)
			{
				ItemShopDisplayEntityView itemShopDisplayEntityView = entityViewsDB.QueryEntityView<ItemShopDisplayEntityView>(entityId);
				itemShopDisplayEntityView.itemShopDisplayComponent.lastRefreshReason = RefreshReason.ShopRefresh;
				itemShopDisplayEntityView.itemShopDisplayComponent.refresh.set_value(true);
			}
		}

		private IEnumerator BuildGUI()
		{
			IEnumerator enumer = _dataSource.LoadData();
			yield return enumer;
			ItemShopResponseData itemShopResponseData = (ItemShopResponseData)enumer.Current;
			FasterListEnumerator<ItemShopDisplayEntityView> enumerator = entityViewsDB.QueryEntityViews<ItemShopDisplayEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ItemShopDisplayEntityView current = enumerator.get_Current();
					IItemShopDisplayComponent itemShopDisplayComponent = current.itemShopDisplayComponent;
					int num = 0;
					int num2 = 0;
					foreach (ItemShopBundle itemShopBundle in itemShopResponseData.ItemShopBundles)
					{
						if (itemShopBundle.Recurrence == ItemShopRecurrence.Daily)
						{
							num2++;
						}
						else
						{
							num++;
						}
						_factory.BuildProduct(itemShopBundle, itemShopBundle.Recurrence, active: true, itemShopDisplayComponent);
					}
					for (int i = num; i < itemShopDisplayComponent.maxShownFeaturedBundles; i++)
					{
						_factory.BuildProduct(null, ItemShopRecurrence.Weekly, active: false, itemShopDisplayComponent);
					}
					for (int j = num2; j < itemShopDisplayComponent.maxShownDailyBundles; j++)
					{
						_factory.BuildProduct(null, ItemShopRecurrence.Daily, active: false, itemShopDisplayComponent);
					}
					for (int k = 0; k < itemShopDisplayComponent.maxShownFeaturedBundles; k++)
					{
						_factory.BuildEmptySlot(ItemShopRecurrence.Weekly, k >= num, itemShopDisplayComponent);
					}
					for (int l = 0; l < itemShopDisplayComponent.maxShownDailyBundles; l++)
					{
						_factory.BuildEmptySlot(ItemShopRecurrence.Daily, l >= num2, itemShopDisplayComponent);
					}
					current.itemShopDisplayComponent.dailyStockHash.set_value(itemShopResponseData.dailyStockHash);
					current.itemShopDisplayComponent.featuredStockHash.set_value(itemShopResponseData.featuredStockHash);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private IEnumerator UpdateExpiryTimeLabels()
		{
			while (true)
			{
				FasterListEnumerator<ItemShopDisplayEntityView> enumerator = entityViewsDB.QueryEntityViews<ItemShopDisplayEntityView>().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ItemShopDisplayEntityView display = enumerator.get_Current();
						if (IsExpired())
						{
							yield return RefreshShopDueToExpiry(display);
						}
						DateTime now = DateTime.UtcNow;
						string dailyRemainingStr = GetTimeRemainingString(_dailyExpiryDT - now);
						string featuredRemainingStr = GetTimeRemainingString(_featuredExpiryDT - now);
						display.itemShopDisplayComponent.remainingDailyTime = dailyRemainingStr;
						display.itemShopDisplayComponent.remainingFeaturedTime = featuredRemainingStr;
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				yield return (object)new WaitForSecondsEnumerator(1f);
			}
		}

		private IEnumerator RefreshShopDueToExpiry(ItemShopDisplayEntityView display)
		{
			_loadingIconPresenter.NotifyLoading("LoadingItemShop");
			display.itemShopDisplayComponent.remainingDailyTime = string.Empty;
			display.itemShopDisplayComponent.remainingFeaturedTime = string.Empty;
			display.itemShopDisplayComponent.lastRefreshReason = RefreshReason.ShopRefresh;
			display.itemShopDisplayComponent.refresh.set_value(true);
			DateTime startTime = DateTime.UtcNow;
			while (display.itemShopDisplayComponent.refresh.get_value() && (DateTime.UtcNow - startTime).TotalMilliseconds < 5000.0)
			{
				yield return null;
			}
			if (!display.itemShopDisplayComponent.refresh.get_value())
			{
				_dailyExpiryDT = GenerateDailyBundleExpiryTime();
				_featuredExpiryDT = GenerateWeeklyBundleExpiryTime();
			}
			_loadingIconPresenter.NotifyLoadingDone("LoadingItemShop");
		}

		private bool IsExpired()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow > _dailyExpiryDT || utcNow > _featuredExpiryDT)
			{
				return true;
			}
			return false;
		}

		private static string GetTimeRemainingString(TimeSpan ts)
		{
			if (ts.TotalDays >= 1.0)
			{
				string arg = (!(ts.TotalDays < 2.0)) ? StringTableBase<StringTable>.Instance.GetString("strDays") : StringTableBase<StringTable>.Instance.GetString("strDay");
				return $"{ts.Days} {arg}";
			}
			if (ts.Hours > 0)
			{
				return string.Format("{0} {1} {2} {3} {4} {5}", ts.Hours, StringTableBase<StringTable>.Instance.GetString("strHrsShort"), ts.Minutes, StringTableBase<StringTable>.Instance.GetString("strMinsShort"), ts.Seconds, StringTableBase<StringTable>.Instance.GetString("strSecondsShort"));
			}
			return string.Format("{0} {1} {2} {3}", ts.Minutes, StringTableBase<StringTable>.Instance.GetString("strMinsShort"), ts.Seconds, StringTableBase<StringTable>.Instance.GetString("strSecondsShort"));
		}

		private static DateTime GenerateDailyBundleExpiryTime()
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 15, 0, 0);
			if (utcNow > dateTime)
			{
				return dateTime.AddDays(1.0);
			}
			return dateTime;
		}

		private static DateTime GenerateWeeklyBundleExpiryTime()
		{
			DateTime utcNow = DateTime.UtcNow;
			int num = (int)(1 - utcNow.DayOfWeek + 7) % 7;
			DateTime dateTime = utcNow.AddDays(num);
			DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 15, 0, 0);
			if (utcNow > dateTime2)
			{
				return dateTime2.AddDays(7.0);
			}
			return dateTime2;
		}
	}
}
