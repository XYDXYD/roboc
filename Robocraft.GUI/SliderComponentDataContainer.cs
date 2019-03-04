using System;

namespace Robocraft.GUI
{
	internal class SliderComponentDataContainer : IGenericComponentDataContainer
	{
		private float _value;

		public SliderComponentDataContainer(float value)
		{
			PackData(value);
		}

		public void PackData<T>(T data)
		{
			if (typeof(T) == typeof(float))
			{
				object value = data;
				_value = Convert.ToSingle(value);
			}
			else
			{
				_value = 0f;
			}
		}

		public T UnpackData<T>()
		{
			if (typeof(T) == typeof(float))
			{
				return (T)Convert.ChangeType(_value, typeof(T));
			}
			_value = 0f;
			return default(T);
		}
	}
}
