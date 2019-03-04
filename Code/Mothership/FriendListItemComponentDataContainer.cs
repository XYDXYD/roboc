using Robocraft.GUI;
using System;

namespace Mothership
{
	internal class FriendListItemComponentDataContainer : IGenericComponentDataContainer
	{
		internal struct ListItemInfo
		{
			internal Friend friend;

			internal int index;

			internal IGenericListEntryView listItemRef;

			internal string buttonClicked;
		}

		private ListItemInfo _listInfo;

		public FriendListItemComponentDataContainer(ListItemInfo data)
		{
			PackData(data);
		}

		public FriendListItemComponentDataContainer(IGenericListEntryView originator, int index_, Friend friend, string buttonClicked)
		{
			PackData(new ListItemInfo
			{
				index = index_,
				listItemRef = originator,
				friend = friend,
				buttonClicked = buttonClicked
			});
		}

		public FriendListItemComponentDataContainer(IGenericListEntryView originator, Friend friend, string buttonClicked)
		{
			PackData(new ListItemInfo
			{
				index = 0,
				friend = friend,
				listItemRef = originator,
				buttonClicked = buttonClicked
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
