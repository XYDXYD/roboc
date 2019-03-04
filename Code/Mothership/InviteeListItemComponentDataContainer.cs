using Robocraft.GUI;
using System;

namespace Mothership
{
	internal class InviteeListItemComponentDataContainer : IGenericComponentDataContainer
	{
		internal enum ListItemAction
		{
			Accept,
			Decline
		}

		internal struct ListItemInfo
		{
			internal string nameOfPlayer;

			internal string nameOfClan;

			internal ListItemAction action;

			internal IGenericListEntryView listItemRef;
		}

		private ListItemInfo _listInfo;

		public InviteeListItemComponentDataContainer(string nameOfPlayer_, string nameOfClan_, ListItemAction action_, IGenericListEntryView listItemRef)
		{
			PackData(new ListItemInfo
			{
				nameOfPlayer = nameOfPlayer_,
				nameOfClan = nameOfClan_,
				action = action_,
				listItemRef = listItemRef
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
