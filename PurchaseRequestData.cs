using Svelto.DataStructures;
using System.Collections.Generic;

public class PurchaseRequestData
{
	public PremiumPurchaseResponse premiumPurchaseResponse;

	public Dictionary<uint, uint> newCubeTotals;

	public Dictionary<string, object> cubesAwarded;

	public ShopItemType ShopItemType
	{
		get;
		private set;
	}

	public int TotalPurchasedCC
	{
		get;
		set;
	}

	public FasterList<int> PurchasedCCList
	{
		get;
		set;
	}

	public PurchaseRequestData(ShopItemType shopItemType)
	{
		ShopItemType = shopItemType;
		PurchasedCCList = new FasterList<int>();
	}
}
