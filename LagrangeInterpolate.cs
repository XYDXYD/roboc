using UnityEngine;

internal sealed class LagrangeInterpolate
{
	public static float Interpolate(float[] fnInputs, float[] fnOutputs, float targetInput)
	{
		if (fnInputs.Length != fnOutputs.Length)
		{
			return 0f;
		}
		int num = fnInputs.Length;
		if (num <= 0)
		{
			return 0f;
		}
		if (num == 1)
		{
			return fnOutputs[0];
		}
		float[] array = new float[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = targetInput - fnInputs[i];
		}
		float num2 = 0f;
		for (int j = 0; j < num; j++)
		{
			float num3 = 1f;
			for (int k = 0; k < num; k++)
			{
				if (j != k)
				{
					if (fnInputs[j] == fnInputs[k])
					{
						return 0f;
					}
					num3 *= array[k] / (fnInputs[j] - fnInputs[k]);
				}
			}
			num2 += num3 * fnOutputs[j];
		}
		return num2;
	}

	public static Vector3 Interpolate(float[] inputTimes, Vector3[] outputValues, float targetTime)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		if (inputTimes.Length != outputValues.Length)
		{
			return Vector3.get_zero();
		}
		int num = inputTimes.Length;
		if (num <= 0)
		{
			return Vector3.get_zero();
		}
		if (num == 1)
		{
			return outputValues[0];
		}
		Vector3[] array = (Vector3[])new Vector3[num];
		for (int i = 0; i < num; i++)
		{
			float num2 = 1f;
			float num3 = 1f;
			for (int j = 0; j < num; j++)
			{
				if (i != j)
				{
					num2 *= targetTime - inputTimes[j];
					num3 *= inputTimes[i] - inputTimes[j];
				}
			}
			float num4 = num2 / num3;
			array[i] = num4 * outputValues[i];
		}
		Vector3 val = Vector3.get_zero();
		Vector3[] array2 = array;
		foreach (Vector3 val2 in array2)
		{
			val += val2;
		}
		return val;
	}

	public static Quaternion Interpolate(float[] inputTimes, Quaternion[] outputValues, float targetTime)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		if (inputTimes.Length != outputValues.Length)
		{
			return Quaternion.get_identity();
		}
		int num = inputTimes.Length;
		if (num <= 0)
		{
			return Quaternion.get_identity();
		}
		if (num == 1)
		{
			return outputValues[0];
		}
		Quaternion[] array = (Quaternion[])new Quaternion[num];
		for (int i = 0; i < num; i++)
		{
			float num2 = 1f;
			float num3 = 1f;
			for (int j = 0; j < num; j++)
			{
				if (i != j)
				{
					num2 *= targetTime - inputTimes[j];
					num3 *= inputTimes[i] - inputTimes[j];
				}
			}
			float num4 = num2 / num3;
			array[i].x = num4 * outputValues[i].x;
			array[i].y = num4 * outputValues[i].y;
			array[i].z = num4 * outputValues[i].z;
			array[i].w = num4 * outputValues[i].w;
		}
		Quaternion result = default(Quaternion);
		result._002Ector(0f, 0f, 0f, 0f);
		Quaternion[] array2 = array;
		for (int k = 0; k < array2.Length; k++)
		{
			Quaternion val = array2[k];
			result.x += val.x;
			result.y += val.y;
			result.z += val.z;
			result.w += val.w;
		}
		return result;
	}
}
