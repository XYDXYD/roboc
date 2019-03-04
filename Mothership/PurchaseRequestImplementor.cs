namespace Mothership
{
	internal class PurchaseRequestImplementor : IPurchaseRequestComponent
	{
		public RealMoneyStoreItemBundle item
		{
			get;
			private set;
		}

		public ShortCutMode previousShortcutMode
		{
			get;
			private set;
		}

		public bool purchaseConfirmed
		{
			get;
			set;
		}

		public PurchaseRequestImplementor(RealMoneyStoreItemBundle item_, ShortCutMode previousShortcutMode_)
		{
			item = item_;
			previousShortcutMode = previousShortcutMode_;
		}
	}
}
