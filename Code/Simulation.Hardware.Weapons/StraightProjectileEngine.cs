using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class StraightProjectileEngine : IPhysicallyTickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		[Inject]
		internal RigidbodyDataContainer rigidbodyDataContainer
		{
			get;
			private set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void PhysicsTick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<ProjectileNode> val = entityViewsDB.QueryEntityViews<ProjectileNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				UpdateProjectile(val.get_Item(i));
			}
		}

		private void UpdateProjectile(ProjectileNode laserProjectile)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			Transform t = laserProjectile.transformComponent.T;
			IProjectileAliveComponent projectileAliveComponent = laserProjectile.projectileAliveComponent;
			if (t.get_gameObject().get_activeInHierarchy() && projectileAliveComponent.active)
			{
				IProjectileMovementStatsComponent projectileMovementStats = laserProjectile.projectileMovementStats;
				IProjectileTimeComponent projectileTimeComponent = laserProjectile.projectileTimeComponent;
				IProjectileRangeComponent projectileRangeComponent = laserProjectile.projectileRangeComponent;
				IProjectileOwnerComponent projectileOwnerComponent = laserProjectile.projectileOwnerComponent;
				float num = Vector3.Distance(projectileMovementStats.startPosition, t.get_position());
				if (projectileAliveComponent.justFired)
				{
					float num2 = Time.get_timeSinceLevelLoad() - projectileTimeComponent.startTime;
					projectileTimeComponent.startTime = Time.get_timeSinceLevelLoad();
					projectileAliveComponent.justFired = false;
					Vector3 position = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, projectileOwnerComponent.machineId).get_position();
					position += rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, projectileOwnerComponent.machineId).get_velocity() * num2;
					Vector3 robotStartPosition = projectileMovementStats.robotStartPosition;
					Vector3 val = position - robotStartPosition;
					IProjectileMovementStatsComponent projectileMovementStatsComponent = projectileMovementStats;
					projectileMovementStatsComponent.startPosition += val;
				}
				float num3 = Time.get_timeSinceLevelLoad() - projectileTimeComponent.startTime;
				if (num > projectileRangeComponent.maxRange || num3 > projectileTimeComponent.maxTime || laserProjectile.projectileStateComponent.disabled)
				{
					DisableProjectile(laserProjectile);
					return;
				}
				CalculatePredictedPosition(num3, laserProjectile);
				t.set_position(CalculatePosition(num3, projectileMovementStats.startPosition, projectileMovementStats.startVelocity));
			}
		}

		private void CalculatePredictedPosition(float timeElapsed, ProjectileNode projectile)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			IProjectileMovementStatsComponent projectileMovementStats = projectile.projectileMovementStats;
			IProjectileTimeComponent projectileTimeComponent = projectile.projectileTimeComponent;
			IPredictedProjectilePositionComponent predictedProjectilePosition = projectile.predictedProjectilePosition;
			predictedProjectilePosition.currentPos = CalculatePosition(timeElapsed, projectileMovementStats.startPosition, projectileMovementStats.startVelocity);
			predictedProjectilePosition.nextPos = CalculatePosition(timeElapsed + Time.get_fixedDeltaTime(), projectileMovementStats.startPosition, projectileMovementStats.startVelocity);
		}

		private Vector3 CalculatePosition(float timeElapsed, Vector3 startPosition, Vector3 startVelocity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = startVelocity * timeElapsed;
			return startPosition + val;
		}

		private void DisableProjectile(ProjectileNode node)
		{
			int value = node.get_ID();
			node.projectileAliveComponent.resetProjectile.Dispatch(ref value);
			node.projectileAliveComponent.active = false;
			node.projectileStateComponent.disabled = false;
		}

		public void Ready()
		{
		}
	}
}
