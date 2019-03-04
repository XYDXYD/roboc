using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using System;

namespace Simulation
{
	internal sealed class HealingReporter
	{
		public event Action<int, int, FasterList<InstantiatedCube>, TargetType> OnPlayerCubesHealed = delegate
		{
		};

		public event Action<int, int, FasterList<InstantiatedCube>, TargetType> OnPlayerCubesRespawned = delegate
		{
		};

		public event Action<HealingData> PlayerHealed = delegate
		{
		};

		public event Action<int, FasterList<InstantiatedCube>, FasterList<InstantiatedCube>, TargetType> OnProtoniumHealingApplied = delegate
		{
		};

		public void PostCubeHealed(HealingData data)
		{
			if (data.targetType == TargetType.Player)
			{
				if (data.respawnedCubes.get_Count() > 0)
				{
					this.OnPlayerCubesRespawned(data.shooterPlayerId, data.targetId, data.respawnedCubes, data.shooterType);
				}
				if (data.healedCubes.get_Count() > 0)
				{
					this.OnPlayerCubesHealed(data.shooterPlayerId, data.targetId, data.healedCubes, data.shooterType);
				}
				if (data.respawnedCubes.get_Count() > 0 || data.healedCubes.get_Count() > 0)
				{
					this.PlayerHealed(data);
				}
			}
			else if (data.targetType == TargetType.TeamBase)
			{
				this.OnProtoniumHealingApplied(data.targetId, data.respawnedCubes, data.healedCubes, data.targetType);
			}
		}
	}
}
