using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Nano
{
	internal sealed class NanoCrosshairUpdaterEngine : ITickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		[Inject]
		internal CrosshairController crosshairController
		{
			private get;
			set;
		}

		[Inject]
		internal MachineRootContainer machineRootContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal NetworkMachineManager networkMachineManager
		{
			private get;
			set;
		}

		[Inject]
		internal RigidbodyDataContainer rigidbodyDataContainer
		{
			private get;
			set;
		}

		[Inject]
		internal CubeHealingPropagator healingPropagator
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		public void Tick(float deltaSec)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			if (crosshairController.GetCrossHairType() == CrosshairType.Nano)
			{
				FasterReadOnlyList<NanoCrosshairUpdaterNode> val = entityViewsDB.QueryEntityViews<NanoCrosshairUpdaterNode>();
				for (int i = 0; i < val.get_Count(); i++)
				{
					NanoCrosshairUpdaterNode nanoCrosshairUpdaterNode = val.get_Item(i);
					if (nanoCrosshairUpdaterNode.ownerComponent.ownedByMe && !nanoCrosshairUpdaterNode.healthStatusComponent.disabled && nanoCrosshairUpdaterNode.weaponActiveComponent.active && !flag)
					{
						flag = CheckAllyCanBeHealed(nanoCrosshairUpdaterNode);
						if (flag)
						{
							break;
						}
					}
				}
			}
			crosshairController.inRange = flag;
		}

		private bool CheckAllyCanBeHealed(NanoCrosshairUpdaterNode node)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = node.muzzleComponent.muzzleFlashLocations[0].get_position();
			Vector3 direction = node.aimingComponent.targetPoint - position;
			WeaponRaycastUtility.Ray ray = default(WeaponRaycastUtility.Ray);
			ray.startPosition = position;
			ray.direction = direction;
			ray.range = node.rangeComponent.maxRange;
			WeaponRaycastUtility.Ray ray2 = ray;
			HitResult[] array = new HitResult[1];
			WeaponRaycastUtility.Parameters parameters = default(WeaponRaycastUtility.Parameters);
			parameters.machineRootContainer = machineRootContainer;
			parameters.playerTeamsContainer = playerTeamsContainer;
			parameters.playerMachinesContainer = playerMachinesContainer;
			parameters.machineManager = networkMachineManager;
			parameters.shooterId = playerTeamsContainer.localPlayerId;
			parameters.isShooterAi = false;
			WeaponRaycastUtility.Parameters parameters2 = parameters;
			WeaponRaycastUtility.RaycastWeaponAim(ref ray2, ref parameters2, array, ignoreTeamMates: false);
			return array[0].hitAlly;
		}
	}
}
