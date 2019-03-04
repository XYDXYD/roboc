using System;

namespace Robocraft.GUI
{
	internal class TextEntryComponentDataContainer : IGenericComponentDataContainer
	{
		private string _entry;

		public TextEntryComponentDataContainer(string entry)
		{
			PackData(entry);
		}

		public void PackData<T>(T data)
		{
			if (typeof(T) == typeof(string))
			{
				_entry = (data as string);
			}
			else
			{
				_entry = string.Empty;
			}
		}

		public T UnpackData<T>()
		{
			if (typeof(T) == typeof(string))
			{
				return (T)Convert.ChangeType(_entry, typeof(T));
			}
			_entry = string.Empty;
			return default(T);
		}
	}
}
