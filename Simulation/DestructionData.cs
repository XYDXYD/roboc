using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using UnityEngine;

namespace Simulation
{
	internal class DestructionData
	{
		public int shooterId;

		public int hitPlayerId;

		public int hitMachineId;

		public bool shooterIsMe;

		public bool targetIsMe;

		public bool targetIsLocal;

		public bool shooterIsLocal;

		public bool isDestroyed;

		public TargetType targetType;

		public FasterList<InstantiatedCube> destroyedCubes;

		public FasterList<InstantiatedCube> damagedCubes;

		public IMachineMap hitMachineMap;

		public Rigidbody hitRigidbody;

		public int weaponDamage;

		public void Reset()
		{
			shooterId = -1;
			hitPlayerId = -1;
			hitMachineId = -1;
			shooterIsMe = false;
			targetIsMe = false;
			targetIsLocal = false;
			shooterIsLocal = false;
			isDestroyed = false;
			targetType = TargetType.Environment;
			destroyedCubes = null;
			damagedCubes = null;
			hitMachineMap = null;
			hitRigidbody = null;
			weaponDamage = 0;
		}
	}
}
