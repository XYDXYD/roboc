using Svelto.IoC;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class WeaponListUtility
	{
		[Inject]
		public ICubeList cubeList
		{
			private get;
			set;
		}

		[Inject]
		public NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		[Inject]
		public MachineRootContainer rootContainer
		{
			private get;
			set;
		}

		[Inject]
		public LivePlayersContainer livePlayers
		{
			private get;
			set;
		}

		[Inject]
		public PlayerMachinesContainer playerMachines
		{
			private get;
			set;
		}

		public GameObject GetWeaponGameObject(Byte3 gridKey, int machineId)
		{
			if (!rootContainer.IsMachineRegistered(TargetType.Player, machineId))
			{
				return null;
			}
			int playerFromMachineId = playerMachines.GetPlayerFromMachineId(TargetType.Player, machineId);
			if (!livePlayers.IsPlayerAlive(TargetType.Player, playerFromMachineId))
			{
				return null;
			}
			IMachineMap machineMap = machineManager.GetMachineMap(TargetType.Player, machineId);
			MachineCell cellAt = machineMap.GetCellAt(gridKey);
			if (cellAt.info.isDestroyed)
			{
				return null;
			}
			return cellAt.gameObject;
		}

		public GameObject GetWeaponGameObjectFast(Byte3 gridKey, int machineId)
		{
			IMachineMap machineMap = machineManager.GetMachineMap(TargetType.Player, machineId);
			MachineCell cellAt = machineMap.GetCellAt(gridKey);
			if (cellAt.info.isDestroyed)
			{
				return null;
			}
			return cellAt.gameObject;
		}
	}
}
