using Services.Analytics;
using Svelto.ServiceLayer;
using System.Collections;
using Utility;

namespace Mothership.ItemShop
{
	internal static class ItemShopAnalyticsUtility
	{
		public static string GetCategory(ItemShopCategory category)
		{
			if (category == ItemShopCategory.Cube)
			{
				return "Cosmetic";
			}
			return category.ToString();
		}

		public static string GetContext(ItemShopRecurrence recurrence)
		{
			switch (recurrence)
			{
			case ItemShopRecurrence.Daily:
				return "ItemShopDaily";
			case ItemShopRecurrence.Weekly:
				return "ItemShopFeatured";
			default:
				Console.LogError("Log Item Bought failed while checking the Recurrence");
				return null;
			}
		}

		public static IEnumerator LogRestockedBundle(ItemShopBundle bundle, RefreshReason reason, IAnalyticsRequestFactory requestFactory)
		{
			LogItemStockedDependency dep = new LogItemStockedDependency(context_: GetContext(bundle.Recurrence), itemType_: GetCategory(bundle.Category), locked_: !bundle.OwnsRequiredCube, item_: bundle.BundleNameStrKey, currency_: bundle.CurrencyType.ToString(), restock_: reason.ToString(), discount_: bundle.GetDiscountPercent());
			TaskService task = requestFactory.Create<ILogItemStockedRequest, LogItemStockedDependency>(dep).AsTask();
			yield return task;
			if (!task.succeeded)
			{
				Console.LogError("Log Item Stocked Request failed. " + task.behaviour.exceptionThrown);
			}
		}

		public static IEnumerator LogVisitedShop(IAnalyticsRequestFactory requestFactory, bool dailyUpdated, bool weeklyUpdated)
		{
			LogItemShopVisitedDependency.Context context = LogItemShopVisitedDependency.Context.None;
			if (weeklyUpdated && dailyUpdated)
			{
				context = LogItemShopVisitedDependency.Context.Both;
			}
			else if (dailyUpdated)
			{
				context = LogItemShopVisitedDependency.Context.Daily;
			}
			else if (weeklyUpdated)
			{
				context = LogItemShopVisitedDependency.Context.Featured;
			}
			LogItemShopVisitedDependency dep = new LogItemShopVisitedDependency(context.ToString());
			TaskService task = requestFactory.Create<ILogItemShopVisitedRequest, LogItemShopVisitedDependency>(dep).AsTask();
			yield return task;
			if (!task.succeeded)
			{
				Console.LogError("Log ItemShop visited Request failed. " + task.behaviour.exceptionThrown);
			}
		}
	}
}
