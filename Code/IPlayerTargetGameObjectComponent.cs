using Simulation;
using UnityEngine;

public interface IPlayerTargetGameObjectComponent
{
	Rigidbody rigidBody
	{
		get;
	}

	int playerId
	{
		get;
	}

	int teamId
	{
		get;
	}

	int machineId
	{
		get;
	}

	float horizontalRadius
	{
		get;
	}

	IMachineVisibilityComponent machineVisibilityComponent
	{
		get;
	}
}
