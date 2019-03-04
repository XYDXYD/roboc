using System;
using System.Collections;

namespace Mothership
{
	internal interface IRealMoneyStoreItemDataSource
	{
		event Action<bool> OnDataChanged;

		RealMoneyStoreItemBundle GetDataItem(int index, RealMoneyStoreSlotDisplayType slotType);

		int GetDataItemsCount(RealMoneyStoreSlotDisplayType slotType);

		IEnumerator LoadData();
	}
}
