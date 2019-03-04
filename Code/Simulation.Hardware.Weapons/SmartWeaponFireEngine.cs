using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class SmartWeaponFireEngine : SingleEntityViewEngine<SmartWeaponFireNode>, IInitialize, IQueryingEntityViewEngine, ITickable, IEngine, ITickableBase
	{
		private SmartWeaponFireNode _firstMisfiredWeapon;

		private WeaponMisfiredAllObservable _misfiredAllObservable;

		private float _misfireAngle;

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public SmartWeaponFireEngine(WeaponMisfiredAllObservable misfiredAllObservable)
		{
			_misfiredAllObservable = misfiredAllObservable;
		}

		void IInitialize.OnDependenciesInjected()
		{
			LoadClientGameSettings();
		}

		private void LoadClientGameSettings()
		{
			IGetGameClientSettingsRequest getGameClientSettingsRequest = serviceFactory.Create<IGetGameClientSettingsRequest>();
			getGameClientSettingsRequest.SetAnswer(new ServiceAnswer<GameClientSettingsDependency>(delegate(GameClientSettingsDependency data)
			{
				_misfireAngle = data.smartRotationMisfireAngle;
			}));
			getGameClientSettingsRequest.Execute();
		}

		private static bool IsWeaponInsideEnvironment(SmartWeaponFireNode node)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			Vector3 muzzlePosition = WeaponAimUtility.GetMuzzlePosition(node.muzzleFlashComponent);
			Vector3 direction = WeaponAimUtility.GetDirection(node.ownerComponent, node.aimingVariablesComponent, node.weaponRotationTransformsComponent, node.weaponAccuracyComponent);
			Vector3 position = node.cubePositionComponent.position;
			Vector3 val = muzzlePosition - position;
			float magnitude = val.get_magnitude();
			Vector3 val2 = muzzlePosition - direction * magnitude;
			return Physics.Raycast(val2, direction, magnitude, (1 << GameLayers.TERRAIN) | (1 << GameLayers.PROPS));
		}

		private static bool IsWeaponGoingToMisfire(SmartWeaponFireNode node, float misfireAngleDegrees)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			Vector3 muzzlePosition = WeaponAimUtility.GetMuzzlePosition(node.muzzleFlashComponent);
			Vector3 direction = WeaponAimUtility.GetDirection(node.ownerComponent, node.aimingVariablesComponent, node.weaponRotationTransformsComponent, node.weaponAccuracyComponent);
			node.projectileCreationComponent.launchDirection = direction;
			IAimingVariablesComponent aimingVariablesComponent = node.aimingVariablesComponent;
			if (aimingVariablesComponent.isBlocked)
			{
				return false;
			}
			if (node.aimingVariablesComponent.changingAimQuickly)
			{
				float num = node.aimingVariablesComponent.currHorizAngle * 57.29578f;
				float num2 = node.aimingVariablesComponent.currVertAngle * 57.29578f;
				float num3 = node.aimingVariablesComponent.targetHorizAngle * 57.29578f;
				float num4 = node.aimingVariablesComponent.targetVertAngle * 57.29578f;
				float num5 = Mathf.DeltaAngle(num, num3);
				float num6 = Mathf.DeltaAngle(num2, num4);
				float num7 = num5 * num5 + num6 * num6;
				if (num7 > misfireAngleDegrees * misfireAngleDegrees)
				{
					return false;
				}
			}
			if (Physics.Raycast(muzzlePosition, direction, 100f, GameLayers.MYSELF_LAYER_MASK))
			{
				return false;
			}
			return true;
		}

		private void FireWeapon(IShootingComponent sender, int weaponID)
		{
			SmartWeaponFireNode smartWeaponFireNode = default(SmartWeaponFireNode);
			if (!entityViewsDB.TryQueryEntityView<SmartWeaponFireNode>(weaponID, ref smartWeaponFireNode))
			{
				return;
			}
			int value = smartWeaponFireNode.get_ID();
			MachineWeaponsBlockedNode machineWeaponsBlockedNode = default(MachineWeaponsBlockedNode);
			if (entityViewsDB.TryQueryEntityView<MachineWeaponsBlockedNode>(smartWeaponFireNode.ownerComponent.machineId, ref machineWeaponsBlockedNode))
			{
				bool flag = IsWeaponInsideEnvironment(smartWeaponFireNode);
				machineWeaponsBlockedNode.machineWeaponsBlockedComponent.lastWeaponShotBlocked = flag;
				if (flag)
				{
					_firstMisfiredWeapon = null;
					return;
				}
			}
			if (IsWeaponGoingToMisfire(smartWeaponFireNode, _misfireAngle) || smartWeaponFireNode == _firstMisfiredWeapon)
			{
				if (smartWeaponFireNode == _firstMisfiredWeapon)
				{
					int machineId = smartWeaponFireNode.ownerComponent.machineId;
					_misfiredAllObservable.Dispatch(ref machineId);
				}
				smartWeaponFireNode.shootingComponent.justFired = true;
				smartWeaponFireNode.shootingComponent.lastFireTime = Time.get_timeSinceLevelLoad();
				smartWeaponFireNode.shootingComponent.shotIsGoingToBeFired.Dispatch(ref value);
				_firstMisfiredWeapon = null;
			}
			else
			{
				if (_firstMisfiredWeapon == null)
				{
					_firstMisfiredWeapon = smartWeaponFireNode;
				}
				SmartWeaponFireNode smartWeaponFireNode2 = smartWeaponFireNode;
				smartWeaponFireNode2.muzzleFlashComponent.activeMuzzleFlash = (smartWeaponFireNode2.muzzleFlashComponent.activeMuzzleFlash + 1) % smartWeaponFireNode2.muzzleFlashComponent.muzzleFlashLocations.Count;
				smartWeaponFireNode.misfireComponent.weaponMisfired.Dispatch(ref value);
			}
		}

		public void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<WeaponShootingNode> enumerator = entityViewsDB.QueryEntityViews<WeaponShootingNode>().GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.get_Current().shootingComponent.justFired = false;
			}
		}

		protected override void Add(SmartWeaponFireNode node)
		{
			node.shootingComponent.shotIsReadyToFire.subscribers += FireWeapon;
		}

		protected override void Remove(SmartWeaponFireNode node)
		{
			node.shootingComponent.shotIsReadyToFire.subscribers -= FireWeapon;
		}

		public void Ready()
		{
		}
	}
}
