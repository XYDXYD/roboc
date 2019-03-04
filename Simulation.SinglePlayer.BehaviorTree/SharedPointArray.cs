using BehaviorDesigner.Runtime;
using System;
using UnityEngine;

namespace Simulation.SinglePlayer.BehaviorTree
{
	[Serializable]
	public class SharedPointArray : SharedVariable<Vector3[]>
	{
		public static implicit operator SharedPointArray(Vector3[] value)
		{
			SharedPointArray sharedPointArray = new SharedPointArray();
			sharedPointArray.set_Value(value);
			return sharedPointArray;
		}
	}
}
