using Svelto.DataStructures;
using Svelto.ECS;
using System;
using UnityEngine;

namespace Simulation
{
	internal class MachineTargetsEngine : SingleEntityViewEngine<MachineTargetsEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly MachinePreloader _machinePreloader;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public MachineTargetsEngine(MachinePreloader machinePreloader)
		{
			_machinePreloader = machinePreloader;
		}

		public void Ready()
		{
		}

		protected override void Add(MachineTargetsEntityView entityView)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			SetTargetInfoData(entityView);
			FasterReadOnlyList<MachineTargetsEntityView> val = entityViewsDB.QueryEntityViews<MachineTargetsEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				MachineTargetsEntityView machineTargetsEntityView = val.get_Item(i);
				if (machineTargetsEntityView.get_ID() != entityView.get_ID())
				{
					if (entityView.ownerTeamComponent.ownerTeamId == machineTargetsEntityView.ownerTeamComponent.ownerTeamId)
					{
						machineTargetsEntityView.machineTargetsComponent.allyTargets.Add(entityView);
						entityView.machineTargetsComponent.allyTargets.Add(machineTargetsEntityView);
					}
					else
					{
						machineTargetsEntityView.machineTargetsComponent.enemyTargets.Add(entityView);
						entityView.machineTargetsComponent.enemyTargets.Add(machineTargetsEntityView);
					}
				}
			}
		}

		protected override void Remove(MachineTargetsEntityView entityView)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<MachineTargetsEntityView> val = entityViewsDB.QueryEntityViews<MachineTargetsEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				MachineTargetsEntityView machineTargetsEntityView = val.get_Item(i);
				if (machineTargetsEntityView.get_ID() != entityView.get_ID())
				{
					if (entityView.ownerTeamComponent.ownerTeamId == machineTargetsEntityView.ownerTeamComponent.ownerTeamId)
					{
						machineTargetsEntityView.machineTargetsComponent.allyTargets.Remove(entityView);
						entityView.machineTargetsComponent.allyTargets.Remove(machineTargetsEntityView);
					}
					else
					{
						machineTargetsEntityView.machineTargetsComponent.enemyTargets.Remove(entityView);
						entityView.machineTargetsComponent.enemyTargets.Remove(machineTargetsEntityView);
					}
				}
			}
		}

		private void SetTargetInfoData(MachineTargetsEntityView entityView)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			PreloadedMachine preloadedMachine = _machinePreloader.GetPreloadedMachine(entityView.machineOwnerComponent.ownerMachineId);
			if (preloadedMachine == null)
			{
				throw new Exception("Couldn't load preloaded machine for entity with machine ID: " + entityView.machineOwnerComponent.ownerMachineId);
			}
			Vector3 val = preloadedMachine.machineInfo.MachineSize * 0.5f;
			val.y = 0f;
			entityView.machineTargetInfoComponent.targetInfo.rigidBody = preloadedMachine.rbData;
			entityView.machineTargetInfoComponent.targetInfo.horizontalRadius = val.get_magnitude();
			entityView.machineTargetInfoComponent.targetInfo.machineMap = preloadedMachine.machineMap;
		}
	}
}
