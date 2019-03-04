using System.Collections.Generic;

namespace Mothership
{
	public class RealMoneyStoreItemBundle
	{
		public string ItemSku;

		public string currencyCode;

		public string currencyString;

		public string oldCurrencyString;

		public float priceForCheck;

		public float oldPriceForCheck;

		public int additionalValue;

		public bool mostPopularFlag;

		public bool bestValueFlag;

		public List<RealMoneyStoreItem> Items;
	}
}
