using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal sealed class IonDistorterProjectileEngine : SingleEntityViewEngine<IonDistorterProjectileNode>, IPhysicallyTickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private FasterList<IonDistorterProjectileNode> _projectilesToRecycle = new FasterList<IonDistorterProjectileNode>();

		[Inject]
		internal RigidbodyDataContainer rigidbodyDataContainer
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		protected override void Add(IonDistorterProjectileNode node)
		{
			DisableProjectile(node);
		}

		protected override void Remove(IonDistorterProjectileNode node)
		{
		}

		public void PhysicsTick(float deltaSec)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			RecycleDeadProjectiles();
			FasterReadOnlyList<IonDistorterProjectileNode> val = entityViewsDB.QueryEntityViews<IonDistorterProjectileNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				UpdateProjectile(val.get_Item(i));
			}
		}

		private void UpdateProjectile(IonDistorterProjectileNode ionDistorterProjectile)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			Transform t = ionDistorterProjectile.transformComponent.T;
			IProjectileTimeComponent projectileTimeComponent = ionDistorterProjectile.projectileTimeComponent;
			IProjectileAliveComponent projectileAliveComponent = ionDistorterProjectile.projectileAliveComponent;
			if (t.get_gameObject().get_activeInHierarchy() && projectileAliveComponent.active)
			{
				if (ionDistorterProjectile.projectileAliveComponent.justFired)
				{
					t.set_position(ionDistorterProjectile.projectileMovementStatsComponent.startPosition);
					t.set_rotation(Quaternion.LookRotation(ionDistorterProjectile.projectileMovementStatsComponent.startDirection));
					CalculateProjectileSpreadDirection(ionDistorterProjectile);
					int value = ionDistorterProjectile.get_ID();
					ionDistorterProjectile.collisionComponent.checkCollision.Dispatch(ref value);
					ionDistorterProjectile.projectileAliveComponent.justFired = false;
				}
				float num = Time.get_timeSinceLevelLoad() - projectileTimeComponent.startTime;
				if (num > 2.5f || ionDistorterProjectile.projectileStateComponent.disabled)
				{
					DisableProjectile(ionDistorterProjectile);
				}
			}
		}

		private void CalculateProjectileSpreadDirection(IonDistorterProjectileNode currentProjectile)
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			currentProjectile.collisionComponent.projectileDirections.FastClear();
			float coneAngle = currentProjectile.coneComponent.coneAngle;
			float maxRange = currentProjectile.projectileRangeComponent.maxRange;
			Transform t = currentProjectile.transformComponent.T;
			int numOfRaycasts = currentProjectile.coneComponent.numOfRaycasts;
			int numOfCircles = currentProjectile.coneComponent.numOfCircles;
			int num = 0;
			for (int i = 1; i <= numOfCircles; i++)
			{
				num += i;
			}
			int num2 = Mathf.CeilToInt(((float)numOfRaycasts - 1f) / (float)num);
			int num3 = num2;
			ParticleSystem val = currentProjectile.coneComponent.bullets[0];
			val.get_transform().set_forward(t.get_forward());
			val.set_startLifetime(maxRange / val.get_startSpeed());
			currentProjectile.collisionComponent.projectileDirections.Add(t.get_forward());
			int num4 = numOfRaycasts - 1;
			for (int j = 1; j <= numOfCircles; j++)
			{
				for (int k = 0; k < num3; k++)
				{
					Vector3 val2 = Quaternion.AngleAxis((float)j * coneAngle / (float)numOfCircles, t.get_up()) * t.get_forward();
					int num5 = 360 / num3;
					num5 *= k;
					val2 = Quaternion.AngleAxis((float)num5, t.get_forward()) * val2;
					int num6 = numOfRaycasts - num4 + k;
					val = currentProjectile.coneComponent.bullets[num6];
					val.get_transform().set_forward(val2);
					val.set_startLifetime(maxRange / val.get_startSpeed());
					currentProjectile.collisionComponent.projectileDirections.Add(val2);
				}
				num4 -= num3;
				num3 = ((num4 < num3 + num2) ? num4 : (num3 + num2));
			}
		}

		private void DisableProjectile(IonDistorterProjectileNode node)
		{
			_projectilesToRecycle.Add(node);
		}

		private void RecycleDeadProjectiles()
		{
			if (_projectilesToRecycle.get_Count() > 0)
			{
				for (int num = _projectilesToRecycle.get_Count() - 1; num >= 0; num--)
				{
					IonDistorterProjectileNode ionDistorterProjectileNode = _projectilesToRecycle.get_Item(num);
					GameObject gameObject = ionDistorterProjectileNode.transformComponent.T.get_gameObject();
					if (gameObject.get_activeInHierarchy())
					{
						gameObject.SetActive(false);
					}
				}
			}
			_projectilesToRecycle.FastClear();
		}

		public void Ready()
		{
		}
	}
}
