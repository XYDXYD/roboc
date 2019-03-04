using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class WeaponAccuracyEngine : SingleEntityViewEngine<MachineInputNode>, ITickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private bool _startedFiring;

		private float _startFireTime;

		private MachineInputNode _machineInputNode;

		[Inject]
		internal CrosshairController crosshairController
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			protected get;
			set;
		}

		protected override void Add(MachineInputNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				_machineInputNode = node;
			}
		}

		protected override void Remove(MachineInputNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				_machineInputNode = null;
			}
		}

		public void Tick(float deltaSec)
		{
			if (_machineInputNode == null)
			{
				return;
			}
			bool pressedFire = _machineInputNode.machineInput.fire1 > 0f;
			UpdatePressedFireStatus(pressedFire);
			int num = default(int);
			WeaponAccuracyNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<WeaponAccuracyNode>(_machineInputNode.get_ID(), ref num);
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				WeaponAccuracyNode weaponAccuracyNode = array[i];
				if (weaponAccuracyNode.disabledComponent.enabled && weaponAccuracyNode.weaponActiveComponent.active)
				{
					UpdateAccuracy(weaponAccuracyNode, pressedFire, _startFireTime);
					num2 = Mathf.Max(num2, weaponAccuracyNode.weaponAccuracyModifier.crosshairAccuracyModifier);
				}
			}
			crosshairController.SetWeaponAccuracy(num2);
		}

		private void UpdatePressedFireStatus(bool pressedFire)
		{
			if (pressedFire)
			{
				if (!_startedFiring)
				{
					_startFireTime = Time.get_timeSinceLevelLoad();
					_startedFiring = true;
				}
			}
			else
			{
				_startedFiring = false;
			}
		}

		protected void UpdateAccuracy(WeaponAccuracyNode weaponAccuracy, bool pressedFire, float startFireTime)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			IWeaponAccuracyStatsComponent weaponAccuracyStats = weaponAccuracy.weaponAccuracyStats;
			IWeaponAccuracyModifierComponent weaponAccuracyModifier = weaponAccuracy.weaponAccuracyModifier;
			CalculateVelocity(weaponAccuracy.weaponMovement, weaponAccuracy.transformComponent.T);
			UpdateAimInaccuracy(weaponAccuracyStats, weaponAccuracyModifier, weaponAccuracy.shootingComponent, pressedFire, startFireTime);
			UpdateMovementInaccuracy(weaponAccuracyStats, weaponAccuracyModifier, weaponAccuracy.weaponMovement.velocity, weaponAccuracy.aimingComponent.sqrRotationVelocity, weaponAccuracy.aimingComponent.changingAimQuickly);
			float baseAirInaccuracy = GetBaseAirInaccuracy(weaponAccuracy.ownerComponent.machineId, weaponAccuracyStats);
			weaponAccuracyModifier.totalAccuracy = weaponAccuracyStats.baseInAccuracyDegrees + baseAirInaccuracy + weaponAccuracyModifier.movementAccuracyModifier * weaponAccuracyStats.movementInAccuracyDegrees + weaponAccuracyModifier.repeatFiringModifier * weaponAccuracyStats.repeatFireInAccuracyTotalDegrees;
			if (weaponAccuracy.ownerComponent.ownedByMe)
			{
				weaponAccuracyModifier.crosshairAccuracyModifier = CalculateCrosshairAccuracyModifier(weaponAccuracyStats, weaponAccuracyModifier, baseAirInaccuracy);
			}
		}

		private float CalculateCrosshairAccuracyModifier(IWeaponAccuracyStatsComponent accuracyStats, IWeaponAccuracyModifierComponent accuracyModifier, float baseAirInaccuracyDegrees)
		{
			float num = accuracyStats.movementInAccuracyDegrees + accuracyStats.repeatFireInAccuracyTotalDegrees + baseAirInaccuracyDegrees;
			if (num <= 0f)
			{
				return 0f;
			}
			float num2 = 0f;
			num2 += accuracyModifier.repeatFiringModifier * (accuracyStats.repeatFireInAccuracyTotalDegrees / num);
			num2 += accuracyModifier.movementAccuracyModifier * (accuracyStats.movementInAccuracyDegrees / num);
			return num2 + baseAirInaccuracyDegrees / num;
		}

		private float GetBaseAirInaccuracy(int machineId, IWeaponAccuracyStatsComponent accuracyStats)
		{
			MachineGroundedNode machineGroundedNode = default(MachineGroundedNode);
			if (entityViewsDB.TryQueryEntityView<MachineGroundedNode>(machineId, ref machineGroundedNode) && !machineGroundedNode.machineGroundedComponent.grounded)
			{
				return accuracyStats.baseAirInaccuracyDegrees;
			}
			return 0f;
		}

		private void UpdateAimInaccuracy(IWeaponAccuracyStatsComponent accuracyStats, IWeaponAccuracyModifierComponent accuracyModifier, IShootingComponent shootingComponent, bool pressedFire, float startFireTime)
		{
			float num = Time.get_timeSinceLevelLoad() - shootingComponent.lastFireTime;
			if (pressedFire || num < accuracyStats.accuracyDecayTime)
			{
				if (accuracyModifier.repeatFiringModifier < 1f && accuracyStats.repeatFireInAccuracyDecayTime > 0f)
				{
					accuracyModifier.repeatFiringModifier = Mathf.Clamp01(accuracyModifier.repeatFiringModifier + Time.get_deltaTime() / accuracyStats.repeatFireInAccuracyDecayTime);
				}
			}
			else if (Time.get_timeSinceLevelLoad() - startFireTime > accuracyStats.accuracyNonRecoverTime && accuracyModifier.repeatFiringModifier > 0f && accuracyStats.repeatFireInAccuracyRecoveryTime > 0f)
			{
				accuracyModifier.repeatFiringModifier = Mathf.Clamp01(accuracyModifier.repeatFiringModifier - Time.get_deltaTime() / accuracyStats.repeatFireInAccuracyRecoveryTime);
			}
			if (shootingComponent.justFired && accuracyStats.repeatFireInAccuracyTotalDegrees > 0f)
			{
				accuracyModifier.repeatFiringModifier = Mathf.Clamp01(accuracyModifier.repeatFiringModifier + accuracyStats.fireInstantAccuracyDecayDegrees / accuracyStats.repeatFireInAccuracyTotalDegrees);
			}
		}

		private void UpdateMovementInaccuracy(IWeaponAccuracyStatsComponent accuracyStats, IWeaponAccuracyModifierComponent accuracyModifier, Vector3 velocity, float sqrRotationVelocity, bool changingAimQuickly)
		{
			float sqrMagnitude = velocity.get_sqrMagnitude();
			float num = 0f;
			bool flag = false;
			if (sqrMagnitude > accuracyStats.movementMaxThresholdSpeed * accuracyStats.movementMaxThresholdSpeed)
			{
				if (accuracyStats.movementInAccuracyDecayTime > 0f)
				{
					num = Mathf.Max(num, Time.get_deltaTime() / accuracyStats.movementInAccuracyDecayTime);
					flag = true;
				}
				else
				{
					num = 0f;
					flag = false;
				}
			}
			else if (sqrMagnitude > accuracyStats.movementMinThresholdSpeed * accuracyStats.movementMinThresholdSpeed && accuracyStats.movementInAccuracyDecayTime > 0f)
			{
				float magnitude = velocity.get_magnitude();
				float num2 = (magnitude - accuracyStats.movementMinThresholdSpeed) / (accuracyStats.movementMaxThresholdSpeed - accuracyStats.movementMinThresholdSpeed);
				if (accuracyModifier.movementAccuracyModifier < num2)
				{
					num = Mathf.Max(num, Time.get_deltaTime() / accuracyStats.movementInAccuracyDecayTime);
					flag = true;
				}
				else
				{
					num = 0f;
					flag = false;
				}
			}
			if (changingAimQuickly)
			{
				if (accuracyStats.quickRotationInAccuracyDecayTime > 0f)
				{
					num = Mathf.Max(num, Time.get_deltaTime() / accuracyStats.quickRotationInAccuracyDecayTime);
					flag = true;
				}
			}
			else if (sqrRotationVelocity > accuracyStats.gunRotationThresholdSlow * accuracyStats.gunRotationThresholdSlow && accuracyStats.slowRotationInAccuracyDecayTime > 0f)
			{
				num = Mathf.Max(num, Time.get_deltaTime() / accuracyStats.slowRotationInAccuracyDecayTime);
				flag = true;
			}
			if (flag)
			{
				accuracyModifier.movementAccuracyModifier = Mathf.Clamp01(accuracyModifier.movementAccuracyModifier + num);
			}
			else if (accuracyStats.movementInAccuracyRecoveryTime > 0f)
			{
				accuracyModifier.movementAccuracyModifier = Mathf.Clamp01(accuracyModifier.movementAccuracyModifier - Time.get_deltaTime() / accuracyStats.movementInAccuracyRecoveryTime);
			}
			else
			{
				accuracyModifier.movementAccuracyModifier = 0f;
			}
		}

		private void CalculateVelocity(IWeaponMovementComponent weaponMovement, Transform T)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			float num = 1f / Time.get_deltaTime();
			Vector3 val = T.get_position() - weaponMovement.lastPosition;
			weaponMovement.lastPosition = T.get_position();
			Vector3 val2 = val * num;
			weaponMovement.velocity = weaponMovement.velocity * 0.5f + val2 * 0.5f;
			Quaternion rotation = T.get_rotation();
			Vector3 val3 = rotation.get_eulerAngles() - weaponMovement.lastRotation;
			Quaternion rotation2 = T.get_rotation();
			weaponMovement.lastRotation = rotation2.get_eulerAngles();
			for (int i = 0; i < 3; i++)
			{
				if (Mathf.Abs(val3.get_Item(i)) > 180f)
				{
					int num2;
					val3.set_Item(num2 = i, val3.get_Item(num2) - 360f * Mathf.Sign(val3.get_Item(i)));
				}
			}
			Vector3 val4 = val3 * num;
			weaponMovement.rotationVelocity = weaponMovement.rotationVelocity * 0.5f + val4 * 0.5f;
		}

		public void Ready()
		{
		}
	}
}
