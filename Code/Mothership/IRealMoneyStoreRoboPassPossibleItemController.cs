namespace Mothership
{
	internal interface IRealMoneyStoreRoboPassPossibleItemController
	{
		void SetDataSource(IRealMoneyStorePossibleRoboPassItemsDataSource dataSource);

		void RegisterView(RealMoneyStoreRoboPassPossibleItemView possibleItemsView, int slotId);
	}
}
