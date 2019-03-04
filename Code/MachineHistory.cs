using System.Collections.Generic;
using UnityEngine;

internal sealed class MachineHistory
{
	internal struct RigidBodyState
	{
		public Vector3 position;

		public Quaternion rotation;
	}

	internal struct MachineState
	{
		public float timeStamp;

		public Dictionary<int, RigidBodyState> states;
	}

	private List<MachineState> history;

	public void AddData(Dictionary<int, RigidBodyState> states, float timeStamp)
	{
	}

	public RigidBodyState GetStateAtTime(float time, int id)
	{
		return default(RigidBodyState);
	}
}
