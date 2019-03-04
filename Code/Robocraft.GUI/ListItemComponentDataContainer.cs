using System;

namespace Robocraft.GUI
{
	internal class ListItemComponentDataContainer : IGenericComponentDataContainer
	{
		internal struct ListItemInfo
		{
			internal int index;

			internal IGenericListEntryView listItemRef;
		}

		private ListItemInfo _listInfo;

		public ListItemComponentDataContainer(ListItemInfo data)
		{
			PackData(data);
		}

		public ListItemComponentDataContainer(IGenericListEntryView originator, int index_)
		{
			PackData(new ListItemInfo
			{
				index = index_,
				listItemRef = originator
			});
		}

		public ListItemComponentDataContainer(IGenericListEntryView originator)
		{
			PackData(new ListItemInfo
			{
				index = 0,
				listItemRef = originator
			});
		}

		public void PackData<T>(T data)
		{
			if (typeof(T) == typeof(ListItemInfo))
			{
				_listInfo = (ListItemInfo)Convert.ChangeType(data, typeof(ListItemInfo));
			}
		}

		public T UnpackData<T>()
		{
			if (typeof(T) == typeof(ListItemInfo))
			{
				return (T)Convert.ChangeType(_listInfo, typeof(T));
			}
			return default(T);
		}
	}
}
