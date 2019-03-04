using Svelto.ServiceLayer;
using System;
using System.Collections;
using Utility;

namespace Robocraft.GUI
{
	internal abstract class DataSourceBase : IDataSource, INotifyDataChanged
	{
		private bool _waitFinished;

		public event Action onAllDataChanged;

		public event Action<int, int> onDataItemChanged;

		public abstract int NumberOfDataItemsAvailable(int dimension);

		public abstract void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed);

		public abstract T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2);

		protected void TriggerAllDataChanged()
		{
			SafeEvent.SafeRaise(this.onAllDataChanged);
		}

		protected void TriggerDataItemChanged(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (this.onDataItemChanged != null)
			{
				this.onDataItemChanged(uniqueIdentifier1, uniqueIdentifier2);
			}
		}

		public virtual void ValidateData(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (uniqueIdentifier1 < 0 || uniqueIdentifier2 != 0)
			{
				throw new InvalidDataIndexException(uniqueIdentifier1, uniqueIdentifier2, GetType().Name);
			}
			if (uniqueIdentifier1 >= NumberOfDataItemsAvailable(0))
			{
				throw new NoMoreDataException(uniqueIdentifier1, uniqueIdentifier2, GetType().Name);
			}
		}

		public virtual IEnumerator RefreshData()
		{
			_waitFinished = true;
			RefreshData(delegate
			{
				_waitFinished = false;
				Console.Log("RefreshData() succeeded");
			}, delegate
			{
				_waitFinished = false;
				Console.LogError("RefreshData() failed!");
			});
			while (_waitFinished)
			{
				yield return null;
			}
		}
	}
}
