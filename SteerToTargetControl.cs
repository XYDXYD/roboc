using System;
using UnityEngine;

public class SteerToTargetControl
{
	private float _kp = 10f;

	private float _kd;

	private float _error;

	private float SIN_1 = 0.0174524058f;

	public SteerToTargetControl()
	{
		TextAsset val = Resources.Load("SteerToTargetControl") as TextAsset;
		string[] separator = new string[3]
		{
			"\r\n",
			"\r",
			"\n"
		};
		string[] separator2 = new string[1]
		{
			"="
		};
		string[] array = val.get_text().Split(separator, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(separator2, StringSplitOptions.RemoveEmptyEntries);
			if (array2[0] == "kp")
			{
				_kp = Convert.ToSingle(array2[1]);
			}
			if (array2[0] == "kd")
			{
				_kd = Convert.ToSingle(array2[1]);
			}
		}
	}

	public float ComputeSteerControl(Vector3 desiredDir, Vector3 velocity, float dt)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		float result = 0f;
		desiredDir.y = 0f;
		float sqrMagnitude = desiredDir.get_sqrMagnitude();
		if (sqrMagnitude > Mathf.Epsilon)
		{
			velocity.y = 0f;
			velocity.Normalize();
			desiredDir /= Mathf.Sqrt(sqrMagnitude);
			float num = Vector3.Dot(Vector3.Cross(desiredDir, velocity), Vector3.get_up());
			if (Vector3.Dot(desiredDir, velocity) < 0f)
			{
				num = Mathf.Sign(num);
			}
			else if (Mathf.Abs(num) < SIN_1)
			{
				num = 0f;
			}
			float num2 = (num - _error) / dt;
			result = (0f - _kp) * num - _kd * num2;
			_error = num;
		}
		return result;
	}

	public float ComputeSteerControl(Vector3 target, Vector3 position, Vector3 forward, float dt)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		float result = 0f;
		Vector3 val = target - position;
		val.y = 0f;
		float sqrMagnitude = val.get_sqrMagnitude();
		if (sqrMagnitude > Mathf.Epsilon)
		{
			forward.y = 0f;
			forward.Normalize();
			val /= Mathf.Sqrt(sqrMagnitude);
			float num = Vector3.Dot(Vector3.Cross(val, forward), Vector3.get_up());
			if (Vector3.Dot(val, forward) < 0f)
			{
				num = Mathf.Sign(num);
			}
			else if (Mathf.Abs(num) < SIN_1)
			{
				num = 0f;
			}
			float num2 = (num - _error) / dt;
			result = (0f - _kp) * num - _kd * num2;
			_error = num;
		}
		return result;
	}
}
