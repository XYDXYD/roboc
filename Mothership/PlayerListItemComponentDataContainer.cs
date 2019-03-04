using Robocraft.GUI;
using System;

namespace Mothership
{
	internal class PlayerListItemComponentDataContainer : IGenericComponentDataContainer
	{
		internal struct PlayerListItemInfo
		{
			internal string displayNameOfPlayer;

			internal string nameOfPlayer;

			internal int index;

			internal IGenericListEntryView listItemRef;

			internal string buttonClicked;
		}

		private PlayerListItemInfo _listInfo;

		public PlayerListItemComponentDataContainer(PlayerListItemInfo data)
		{
			PackData(data);
		}

		public PlayerListItemComponentDataContainer(IGenericListEntryView originator, int index_, string playerName, string playerDisplayName, string buttonClicked)
		{
			PackData(new PlayerListItemInfo
			{
				index = index_,
				listItemRef = originator,
				nameOfPlayer = playerName,
				displayNameOfPlayer = playerDisplayName,
				buttonClicked = buttonClicked
			});
		}

		public PlayerListItemComponentDataContainer(IGenericListEntryView originator, string playerName, string playerDisplayName)
		{
			PackData(new PlayerListItemInfo
			{
				index = 0,
				nameOfPlayer = playerName,
				displayNameOfPlayer = playerDisplayName,
				listItemRef = originator
			});
		}

		public void PackData<T>(T data)
		{
			if (typeof(T) == typeof(PlayerListItemInfo))
			{
				_listInfo = (PlayerListItemInfo)Convert.ChangeType(data, typeof(PlayerListItemInfo));
			}
		}

		public T UnpackData<T>()
		{
			if (typeof(T) == typeof(PlayerListItemInfo))
			{
				return (T)Convert.ChangeType(_listInfo, typeof(T));
			}
			return default(T);
		}
	}
}
