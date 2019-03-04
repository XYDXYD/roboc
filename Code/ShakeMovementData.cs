using System;
using System.Collections.Generic;
using UnityEngine;

public class ShakeMovementData
{
	public enum MovementType
	{
		Translation,
		Rotation
	}

	public float startingTime;

	public Vector3 direction;

	public float magnitude;

	public MovementCurve curves;

	public float duration;

	public Random random;

	public Dictionary<int, Vector3> previousMovementValues;

	public MovementType type;

	public bool available;

	public ShakeMovementData(float startingTime, Vector3 direction, float magnitude, MovementCurve curves, float duration, Random random, MovementType type, bool available)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		this.random = random;
		previousMovementValues = new Dictionary<int, Vector3>();
		UpdateValues(startingTime, direction, magnitude, curves, duration, type, available);
	}

	public void UpdateValues(float startingTime, Vector3 direction, float magnitude, MovementCurve curves, float duration, MovementType type, bool available)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		this.startingTime = startingTime;
		this.direction = direction;
		this.magnitude = magnitude;
		this.curves = curves;
		this.duration = duration;
		this.type = type;
		this.available = available;
		previousMovementValues.Clear();
	}
}
