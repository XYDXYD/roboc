using System;
using UnityEngine;

internal sealed class ValueFollower
{
	private float _kp;

	private float _maxSpeed;

	private float _speed;

	public ValueFollower(float kp, float maxSpeed)
	{
		_kp = kp;
		_maxSpeed = maxSpeed;
	}

	public void SetMaxSpeed(float maxSpeed)
	{
		_maxSpeed = maxSpeed;
	}

	public void Reset()
	{
		_speed = 0f;
	}

	public bool Update(float dt, float currentValue, float desiredValue, out float updatedValue)
	{
		bool result = false;
		float num = currentValue - desiredValue;
		float num2 = Math.Min(1f / dt, _kp);
		float num3 = (0f - num) * num2;
		if (Math.Abs(num3) > _maxSpeed)
		{
			num3 = _maxSpeed * Math.Abs(num3) / num3;
			float num4 = (0f - (_speed - num3)) * num2;
			num3 = num4 * dt + _speed;
		}
		else if ((double)Math.Abs(num3) < 0.25 * (double)_maxSpeed && Mathf.Abs(num3 - _speed) / dt < 0.18f)
		{
			result = true;
		}
		updatedValue = currentValue + num3 * dt;
		_speed = num3;
		return result;
	}
}
