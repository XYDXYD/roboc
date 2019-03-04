using UnityEngine;

namespace Simulation.SinglePlayer
{
	internal class PlayerTargetGameObject : IPlayerTargetGameObjectComponent
	{
		public Rigidbody rigidBody
		{
			get;
			private set;
		}

		public int playerId
		{
			get;
			private set;
		}

		public int teamId
		{
			get;
			private set;
		}

		public int machineId
		{
			get;
			private set;
		}

		public float horizontalRadius
		{
			get;
			private set;
		}

		public IMachineMap machineMap
		{
			get;
			private set;
		}

		public IMachineVisibilityComponent machineVisibilityComponent
		{
			get;
			private set;
		}

		public PlayerTargetGameObject(Rigidbody rb, int inGameId, int teamId, int machineId, float horizontalRadius, IMachineMap machineMap, IMachineVisibilityComponent machineVisibilityComponent)
		{
			rigidBody = rb;
			playerId = inGameId;
			this.teamId = teamId;
			this.machineId = machineId;
			this.horizontalRadius = horizontalRadius;
			this.machineMap = machineMap;
			this.machineVisibilityComponent = machineVisibilityComponent;
		}
	}
}
