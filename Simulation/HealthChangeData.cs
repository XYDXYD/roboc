using Simulation.Hardware.Weapons;

namespace Simulation
{
	internal struct HealthChangeData
	{
		public TargetType shooterType;

		public int shooterId;

		public bool shooterIsMe;

		public TargetType targetType;

		public int targetId;

		public int hitMachineId;

		public bool targetIsMe;

		public int deltaHealth;

		public bool isTargetDead;

		public bool IsCriticalHit;
	}
}
