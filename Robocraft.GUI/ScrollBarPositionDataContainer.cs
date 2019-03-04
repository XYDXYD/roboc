using System;

namespace Robocraft.GUI
{
	internal class ScrollBarPositionDataContainer : IGenericComponentDataContainer
	{
		private float _entry;

		public ScrollBarPositionDataContainer(float scrollBarPosition)
		{
			PackData(scrollBarPosition);
		}

		public void PackData<T>(T data)
		{
			if (data.GetType() == typeof(float))
			{
				object value = data;
				_entry = Convert.ToSingle(value);
			}
			else
			{
				_entry = 0f;
			}
		}

		public T UnpackData<T>()
		{
			if (typeof(T) == typeof(float))
			{
				return (T)Convert.ChangeType(_entry, typeof(T));
			}
			_entry = 0f;
			return default(T);
		}
	}
}
