namespace Mothership
{
	internal interface IRealMoneyStoreCardController
	{
		void SetDataSource(IRealMoneyStoreItemDataSource dataSource);

		void RegisterView(RealMoneyStoreItemCardView guiView, int slotId, RealMoneyStoreSlotDisplayType viewType);
	}
}
