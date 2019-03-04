using System;

internal class ItemShopBundle : IComparable<ItemShopBundle>, IEquatable<ItemShopBundle>
{
	public readonly string SKU;

	public readonly string BundleNameStrKey;

	public readonly string SpriteName;

	public readonly bool IsSpriteFullSize;

	public readonly ItemShopCategory Category;

	public readonly int Price;

	public readonly int DiscountPrice;

	public readonly CurrencyType CurrencyType;

	public readonly ItemShopRecurrence Recurrence;

	public readonly bool OwnsRequiredCube;

	public readonly bool Discounted;

	public readonly bool LimitedEdition;

	public ItemShopBundle(string sku, string productNameStrKey, string spriteName, bool isSpriteFullSize, ItemShopCategory category, CurrencyType currencyType, int price, int discountPrice, ItemShopRecurrence recurrence, bool ownsRequiredCube, bool discounted, bool limitedEdition)
	{
		SKU = sku;
		BundleNameStrKey = productNameStrKey;
		SpriteName = spriteName;
		IsSpriteFullSize = isSpriteFullSize;
		Category = category;
		CurrencyType = currencyType;
		Price = price;
		DiscountPrice = discountPrice;
		Recurrence = recurrence;
		OwnsRequiredCube = ownsRequiredCube;
		Discounted = discounted;
		LimitedEdition = limitedEdition;
	}

	public int GetDiscountPercent()
	{
		if (DiscountPrice == Price)
		{
			return 0;
		}
		if (DiscountPrice > Price)
		{
			throw new Exception("Discount price is higher than regular price");
		}
		float num = 1f - (float)DiscountPrice / (float)Price;
		return (int)(100f * num);
	}

	internal int GetFinalPrice()
	{
		if (Discounted)
		{
			return DiscountPrice;
		}
		return Price;
	}

	public override int GetHashCode()
	{
		int num = 17;
		num = num * 23 + SKU.GetHashCode();
		num = num * 23 + GetFinalPrice().GetHashCode();
		num = num * 23 + OwnsRequiredCube.GetHashCode();
		return num * 23 + CurrencyType.GetHashCode();
	}

	public int CompareTo(ItemShopBundle other)
	{
		if (other == this)
		{
			return 0;
		}
		if (OwnsRequiredCube != other.OwnsRequiredCube)
		{
			if (OwnsRequiredCube)
			{
				return -1;
			}
			return 1;
		}
		if (CurrencyType != other.CurrencyType)
		{
			if (CurrencyType == CurrencyType.CosmeticCredits)
			{
				return -1;
			}
			return 1;
		}
		return SKU.CompareTo(other.SKU);
	}

	public bool Equals(ItemShopBundle other)
	{
		return SKU == other.SKU;
	}
}
