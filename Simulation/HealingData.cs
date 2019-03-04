using Simulation.Hardware.Weapons;
using Svelto.DataStructures;

namespace Simulation
{
	internal struct HealingData
	{
		public TargetType shooterType;

		public int shooterPlayerId;

		public TargetType targetType;

		public int targetId;

		public int hitMachineId;

		public FasterList<InstantiatedCube> respawnedCubes;

		public FasterList<InstantiatedCube> healedCubes;
	}
}
