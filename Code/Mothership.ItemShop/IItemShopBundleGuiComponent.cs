namespace Mothership.ItemShop
{
	public interface IItemShopBundleGuiComponent
	{
		string nameText
		{
			set;
		}

		string categoryText
		{
			set;
		}

		string costText
		{
			set;
		}

		CurrencyType currencyType
		{
			set;
		}

		bool isFullSizeSprite
		{
			set;
		}

		string spriteName
		{
			set;
		}

		bool limitedEdition
		{
			set;
		}

		bool discounted
		{
			set;
		}

		string discountPercentText
		{
			set;
		}

		string discountDaysLeftText
		{
			set;
		}

		string nonDiscountedCostText
		{
			set;
		}

		bool locked
		{
			set;
		}
	}
}
