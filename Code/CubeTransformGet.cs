using System;
using System.Collections;
using UnityEngine;

internal sealed class CubeTransformGet
{
	public static void GetCorrectTransform(ref Transform a, ref Transform b)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = a.get_position();
		Vector3 position2 = b.get_position();
		if (a.GetComponent<Joint>() != null)
		{
			GetClosestChild(a, position2);
		}
		if (a.GetComponent<Joint>() != null)
		{
			GetClosestChild(b, position);
		}
	}

	public static Transform GetCorrectChildTransform(Transform cube, Vector3 direction)
	{
		return cube;
	}

	private static Transform GetClosestChild(Transform fromTransform, Vector3 toPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(fromTransform.get_position(), toPosition);
		Transform result = fromTransform;
		IEnumerator enumerator = fromTransform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform val = enumerator.Current;
				float num2 = Vector3.Distance(val.get_position(), toPosition);
				if (num2 < num)
				{
					num = num2;
					result = val;
				}
			}
			return result;
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}
}
