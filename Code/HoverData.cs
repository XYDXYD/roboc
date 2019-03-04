using System;
using System.Collections.Generic;

internal class HoverData : IMovementCategoryData
{
	public float maxHoverHeight
	{
		get;
		private set;
	}

	public float heightTolerance
	{
		get;
		private set;
	}

	public float forceYOffset
	{
		get;
		private set;
	}

	public float heightChangeSpeed
	{
		get;
		private set;
	}

	public float turnTorque
	{
		get;
		private set;
	}

	public float turningScale
	{
		get;
		private set;
	}

	public float smallAngleTurningScale
	{
		get;
		private set;
	}

	public float maxVerticalVelocity
	{
		get;
		private set;
	}

	public float hoverDamping
	{
		get;
		private set;
	}

	public float acceleration
	{
		get;
		private set;
	}

	public float maxAngularVelocity
	{
		get;
		private set;
	}

	public float angularDamping
	{
		get;
		private set;
	}

	public float lateralDamping
	{
		get;
		private set;
	}

	public HoverData(Dictionary<string, object> movementStat)
	{
		maxHoverHeight = GetProperty(movementStat, "maxHoverHeight");
		heightTolerance = GetProperty(movementStat, "heightTolerance");
		forceYOffset = GetProperty(movementStat, "forceYOffset");
		heightChangeSpeed = GetProperty(movementStat, "heightChangeSpeed");
		turnTorque = GetProperty(movementStat, "turnTorque");
		turningScale = GetProperty(movementStat, "turningScale");
		smallAngleTurningScale = GetProperty(movementStat, "smallAngleTurningScale");
		maxVerticalVelocity = GetProperty(movementStat, "verticalTopSpeed");
		hoverDamping = GetProperty(movementStat, "hoverDamping");
		acceleration = GetProperty(movementStat, "acceleration");
		maxAngularVelocity = GetProperty(movementStat, "maxAngularVelocity");
		angularDamping = GetProperty(movementStat, "angularDamping");
		lateralDamping = GetProperty(movementStat, "lateralDamping");
	}

	private float GetProperty(Dictionary<string, object> movementStat, string property)
	{
		if (movementStat.ContainsKey(property))
		{
			return (float)Convert.ToDouble(movementStat[property]);
		}
		throw new Exception("Necessary hover value not found");
	}
}
