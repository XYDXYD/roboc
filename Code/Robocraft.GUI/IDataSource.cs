using Svelto.ServiceLayer;
using System;
using System.Collections;

namespace Robocraft.GUI
{
	internal interface IDataSource
	{
		int NumberOfDataItemsAvailable(int dimension);

		T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2);

		void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed);

		IEnumerator RefreshData();
	}
}
