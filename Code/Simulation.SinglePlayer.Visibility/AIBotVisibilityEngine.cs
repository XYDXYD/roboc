using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.SinglePlayer.Visibility
{
	internal class AIBotVisibilityEngine : IQueryingEntityViewEngine, IInitialize, IWaitForFrameworkInitialization, IEngine
	{
		private WeaponRaycastUtility.Parameters _visibilityRaycastParameters;

		private const float RADIUS_OFFSET_MULTIPLIER = 1.01f;

		[Inject]
		public NetworkMachineManager networkMachineManager
		{
			get;
			set;
		}

		[Inject]
		public MachineRootContainer machineRootContainer
		{
			get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		[Inject]
		public PlayerMachinesContainer playerMachinesContainer
		{
			get;
			set;
		}

		[Inject]
		public LocalAIsContainer localAIsContainer
		{
			get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
		}

		public void OnDependenciesInjected()
		{
			_visibilityRaycastParameters.machineManager = networkMachineManager;
			_visibilityRaycastParameters.machineRootContainer = machineRootContainer;
			_visibilityRaycastParameters.playerMachinesContainer = playerMachinesContainer;
			_visibilityRaycastParameters.playerTeamsContainer = playerTeamsContainer;
			_visibilityRaycastParameters.fusionShieldTag = WeaponRaycastUtility.ALLY_FUSION_SHIELD_TAG;
		}

		public void OnFrameworkInitialized()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				FasterReadOnlyList<MachineTargetsEntityView> playerMachines = entityViewsDB.QueryEntityViews<MachineTargetsEntityView>();
				for (int i = 0; i < playerMachines.get_Count(); i++)
				{
					MachineTargetsEntityView machineTargetsEntityView = playerMachines.get_Item(i);
					machineTargetsEntityView.machineTargetsComponent.visibleTargets.FastClear();
					if (!machineTargetsEntityView.machineOwnerComponent.ownedByMe)
					{
						ProcessAllEnemyTargets(machineTargetsEntityView);
					}
				}
				yield return null;
			}
		}

		private void ProcessAllEnemyTargets(MachineTargetsEntityView machine)
		{
			FasterList<MachineTargetsEntityView> enemyTargets = machine.machineTargetsComponent.enemyTargets;
			for (int i = 0; i < enemyTargets.get_Count(); i++)
			{
				MachineTargetsEntityView machineTargetsEntityView = enemyTargets.get_Item(i);
				if (IsTargetMachineVisible(machineTargetsEntityView) && CheckLineOfSightAgainst(machine, machineTargetsEntityView))
				{
					machine.machineTargetsComponent.visibleTargets.Add(machineTargetsEntityView);
				}
				else
				{
					machine.machineTargetsComponent.visibleTargets.Remove(machineTargetsEntityView);
				}
			}
		}

		private bool IsTargetMachineVisible(MachineTargetsEntityView machine)
		{
			return machine.machineVisibilityComponent.isVisible;
		}

		private bool CheckLineOfSightAgainst(MachineTargetsEntityView playerMachine, MachineTargetsEntityView targetMachine)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			HitResult hitResult = default(HitResult);
			_visibilityRaycastParameters.isShooterAi = IsShooterAI(playerMachine);
			_visibilityRaycastParameters.shooterId = playerMachine.machineOwnerComponent.ownerId;
			float horizontalRadius = playerMachine.machineTargetInfoComponent.targetInfo.horizontalRadius;
			Vector3 worldCenterOfMass = playerMachine.rigidBodyComponent.rb.get_worldCenterOfMass();
			Vector3 worldCenterOfMass2 = targetMachine.rigidBodyComponent.rb.get_worldCenterOfMass();
			Vector3 val = worldCenterOfMass2 - worldCenterOfMass;
			float magnitude = val.get_magnitude();
			val /= magnitude;
			WeaponRaycastUtility.Ray ray = default(WeaponRaycastUtility.Ray);
			ray.startPosition = worldCenterOfMass + val * horizontalRadius * 1.01f;
			ray.direction = val;
			ray.range = magnitude;
			if (WeaponRaycastUtility.RaycastWeaponAim(ref ray, ref _visibilityRaycastParameters, ref hitResult) && hitResult.targetType == TargetType.Player)
			{
				int playerFromMachineId = _visibilityRaycastParameters.playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, hitResult.hitTargetMachineId);
				if (playerFromMachineId == targetMachine.machineOwnerComponent.ownerId)
				{
					flag = true;
				}
			}
			if (flag)
			{
				ray.startPosition = worldCenterOfMass2;
				ray.direction = -val;
				ray.range = magnitude;
				if (Physics.Raycast(ray.startPosition, ray.direction, ray.range, (1 << GameLayers.TERRAIN) | (1 << GameLayers.PROPS)))
				{
					flag = false;
				}
			}
			return flag;
		}

		private bool IsShooterAI(MachineTargetsEntityView machine)
		{
			return localAIsContainer.IsAIHostedLocally(machine.machineOwnerComponent.ownerId);
		}
	}
}
