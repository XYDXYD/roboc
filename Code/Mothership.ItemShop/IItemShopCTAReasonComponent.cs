namespace Mothership.ItemShop
{
	internal interface IItemShopCTAReasonComponent
	{
		bool dailyRestock
		{
			get;
			set;
		}

		bool featuredRestock
		{
			get;
			set;
		}
	}
}
