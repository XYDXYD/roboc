namespace Mothership
{
	internal interface IPurchaseRequestComponent
	{
		RealMoneyStoreItemBundle item
		{
			get;
		}

		ShortCutMode previousShortcutMode
		{
			get;
		}

		bool purchaseConfirmed
		{
			get;
			set;
		}
	}
}
