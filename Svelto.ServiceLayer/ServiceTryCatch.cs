using System;

namespace Svelto.ServiceLayer
{
	public static class ServiceTryCatch
	{
		public static void TryCatch(this object obj, Action action, Action succeeded, Action<Exception> failed)
		{
			try
			{
				action();
			}
			catch (Exception obj2)
			{
				failed?.Invoke(obj2);
				return;
			}
			succeeded();
		}
	}
}
