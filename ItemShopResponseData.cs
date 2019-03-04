using System.Collections.Generic;

internal class ItemShopResponseData
{
	public int dailyStockHash;

	public int featuredStockHash;

	public List<ItemShopBundle> ItemShopBundles
	{
		get;
		set;
	}

	public ItemShopResponseData(List<ItemShopBundle> itemShopBundles, int dailyHash, int featuredHash)
	{
		ItemShopBundles = itemShopBundles;
		dailyStockHash = dailyHash;
		featuredStockHash = featuredHash;
	}

	public void SetSortedItemShopBundles(List<ItemShopBundle> bundles)
	{
		ItemShopBundles = bundles;
	}
}
