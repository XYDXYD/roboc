using Robocraft.GUI;
using System;

namespace Mothership
{
	internal class ClanListItemComponentDataContainer : IGenericComponentDataContainer
	{
		internal struct ClanListItemInfo
		{
			internal string nameOfClan;

			internal int index;

			internal IGenericListEntryView listItemRef;
		}

		private ClanListItemInfo _listInfo;

		public ClanListItemComponentDataContainer(ClanListItemInfo data)
		{
			PackData(data);
		}

		public ClanListItemComponentDataContainer(IGenericListEntryView originator, int index_, string clanName)
		{
			PackData(new ClanListItemInfo
			{
				index = index_,
				listItemRef = originator,
				nameOfClan = clanName
			});
		}

		public ClanListItemComponentDataContainer(IGenericListEntryView originator, string clanName)
		{
			PackData(new ClanListItemInfo
			{
				index = 0,
				nameOfClan = clanName,
				listItemRef = originator
			});
		}

		public void PackData<T>(T data)
		{
			if (typeof(T) == typeof(ClanListItemInfo))
			{
				_listInfo = (ClanListItemInfo)Convert.ChangeType(data, typeof(ClanListItemInfo));
			}
		}

		public T UnpackData<T>()
		{
			if (typeof(T) == typeof(ClanListItemInfo))
			{
				return (T)Convert.ChangeType(_listInfo, typeof(T));
			}
			return default(T);
		}
	}
}
