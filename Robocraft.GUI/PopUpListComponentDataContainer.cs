using SocialServiceLayer;
using System;

namespace Robocraft.GUI
{
	internal class PopUpListComponentDataContainer : IGenericComponentDataContainer
	{
		private ClanType _entry;

		public PopUpListComponentDataContainer(ClanType entry)
		{
			PackData(entry);
		}

		public void PackData<T>(T data)
		{
			if (typeof(T) == typeof(ClanType))
			{
				_entry = (ClanType)(object)data;
			}
		}

		public T UnpackData<T>()
		{
			if (typeof(T) == typeof(ClanType))
			{
				return (T)Convert.ChangeType(_entry, typeof(T));
			}
			return default(T);
		}
	}
}
