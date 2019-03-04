using UnityEngine;

namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal sealed class RocketLauncherProjectileMonoBehaviour : BaseProjectileMonoBehaviour, IHomingProjectileStatsComponent, ILockOnTargetComponent, ISplashDamageComponent
	{
		public float ConeAngle = 30f;

		public int AdditionalHits = 4;

		private int _targetPlayerId;

		private int _targetMachineId;

		private Byte3 _lockedCubePosition;

		Vector3 IHomingProjectileStatsComponent.angularVelocity
		{
			get;
			set;
		}

		float IHomingProjectileStatsComponent.maxRotationAccelerationRad
		{
			get;
			set;
		}

		float IHomingProjectileStatsComponent.maxRotationSpeedRad
		{
			get;
			set;
		}

		float IHomingProjectileStatsComponent.initialRotationSpeedRad
		{
			get;
			set;
		}

		int ILockOnTargetComponent.targetPlayerId
		{
			get
			{
				return _targetPlayerId;
			}
		}

		int ILockOnTargetComponent.targetMachineId
		{
			get
			{
				return _targetMachineId;
			}
		}

		Byte3 ILockOnTargetComponent.lockedCubePosition
		{
			get
			{
				return _lockedCubePosition;
			}
		}

		bool ILockOnTargetComponent.hasAcquiredLock
		{
			get;
			set;
		}

		public float coneAngle => ConeAngle;

		public int additionalHits => AdditionalHits;

		public float damageRadius
		{
			get;
			set;
		}

		internal override void SetGenericProjectileParamaters(WeaponShootingNode weapon, Vector3 launchPosition, Vector3 direction, Vector3 robotStartPos, bool isLocal)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			RocketLauncherShootingNode rocketLauncherShootingNode = (RocketLauncherShootingNode)weapon;
			IHomingProjectileStatsComponent homingProjectileStats = rocketLauncherShootingNode.homingProjectileStats;
			((ISplashDamageComponent)this).damageRadius = rocketLauncherShootingNode.splashDamageStats.damageRadius;
			((IHomingProjectileStatsComponent)this).maxRotationAccelerationRad = homingProjectileStats.maxRotationAccelerationRad;
			((IHomingProjectileStatsComponent)this).maxRotationSpeedRad = homingProjectileStats.maxRotationSpeedRad;
			((IHomingProjectileStatsComponent)this).initialRotationSpeedRad = homingProjectileStats.initialRotationSpeedRad;
			base.SetGenericProjectileParamaters(weapon, launchPosition, direction, robotStartPos, isLocal);
		}

		internal void SetProjectileParamaters(LockOnData lockOnData)
		{
			_targetPlayerId = lockOnData.targetPlayerId;
			_targetMachineId = lockOnData.targetMachineId;
			_lockedCubePosition = lockOnData.lockedCubePosition;
			((ILockOnTargetComponent)this).hasAcquiredLock = lockOnData.hasAcquiredLock;
		}
	}
}
