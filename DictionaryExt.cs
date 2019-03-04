using Svelto.DataStructures;
using System.Collections.Generic;

internal static class DictionaryExt
{
	public static TKey[] ToArray<TKey, TValue>(this Dictionary<TKey, TValue>.KeyCollection keys)
	{
		TKey[] array = new TKey[keys.Count];
		keys.CopyTo(array, 0);
		return array;
	}

	public static TValue[] ToArray<TKey, TValue>(this Dictionary<TKey, TValue>.ValueCollection values)
	{
		TValue[] array = new TValue[values.Count];
		values.CopyTo(array, 0);
		return array;
	}

	public static TKey[] ToArray<TKey, TValue>(this KeyCollection<TKey, TValue> keys)
	{
		TKey[] array = new TKey[keys.get_Count()];
		keys.CopyTo(array, 0);
		return array;
	}

	public static TValue[] ToArray<TKey, TValue>(this ValueCollection<TKey, TValue> values)
	{
		TValue[] array = new TValue[values.get_Count()];
		values.CopyTo(array, 0);
		return array;
	}
}
