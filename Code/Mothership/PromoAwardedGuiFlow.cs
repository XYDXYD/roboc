using Svelto.IoC;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Mothership
{
	internal sealed class PromoAwardedGuiFlow
	{
		[Inject]
		public ICurrenciesTracker currencyTracker
		{
			private get;
			set;
		}

		[Inject]
		public AwardedItemsController awardedItemsController
		{
			private get;
			set;
		}

		[Inject]
		public ReloadRobopassObservable reloadRobopassObservable
		{
			private get;
			set;
		}

		public IEnumerator StartGuiFlowAsTask(Dictionary<string, object> cubesAwarded, long cosmeticCreditsAwarded, bool applyPromoCodeRoboPass)
		{
			if (cubesAwarded.Count > 0)
			{
				yield return awardedItemsController.ShowScreenAndWaitUntilFinished(cubesAwarded);
			}
			if (cosmeticCreditsAwarded > 0 || applyPromoCodeRoboPass)
			{
				yield return currencyTracker.RefreshUserWalletEnumerator();
				if (applyPromoCodeRoboPass)
				{
					reloadRobopassObservable.Dispatch();
				}
			}
			Console.Log("finished promo gui flow");
		}
	}
}
