using System;

namespace PlayMaker
{
	public class PlaymakerDataprovidedResult<T> : IPlaymakerDataprovidedResult
	{
		private T _result;

		Type IPlaymakerDataprovidedResult.ReturnType
		{
			get
			{
				return typeof(T);
			}
		}

		public T Result => _result;

		public void SetResult(T param)
		{
			_result = param;
		}
	}
}
