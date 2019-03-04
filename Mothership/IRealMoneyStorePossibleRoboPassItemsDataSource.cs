using Services.Web.Photon;
using System;
using System.Collections;

namespace Mothership
{
	internal interface IRealMoneyStorePossibleRoboPassItemsDataSource
	{
		event Action<bool> OnDataChanged;

		IEnumerator LoadData();

		int GetDataItemsCount();

		RoboPassPreviewItemDisplayData GetDataItem(int index);
	}
}
