using Simulation.Hardware.Weapons.Plasma;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Mortar
{
	internal sealed class MortarProjectileMonoBehaviour : BaseProjectileMonoBehaviour, IPlasmaProjectileStatsComponent, ISplashDamageComponent
	{
		public float ConeAngle = 30f;

		public int AdditionalHits = 4;

		float IPlasmaProjectileStatsComponent.explosionRadius
		{
			get;
			set;
		}

		float IPlasmaProjectileStatsComponent.timeToFullDamage
		{
			get;
			set;
		}

		float IPlasmaProjectileStatsComponent.startingRadiusScale
		{
			get;
			set;
		}

		float IPlasmaProjectileStatsComponent.currentExplosionRadius
		{
			get;
			set;
		}

		public float damageRadius
		{
			get;
			set;
		}

		public float coneAngle => ConeAngle;

		public int additionalHits => AdditionalHits;

		protected override void Awake()
		{
			base.Awake();
		}

		internal override void SetGenericProjectileParamaters(WeaponShootingNode weapon, Vector3 launchPosition, Vector3 direction, Vector3 robotStartPos, bool isLocal)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			IPlasmaProjectileStatsComponent plasmaProjectileStats = (weapon as PlasmaWeaponShootingNode).plasmaProjectileStats;
			((IPlasmaProjectileStatsComponent)this).explosionRadius = plasmaProjectileStats.explosionRadius;
			((IPlasmaProjectileStatsComponent)this).timeToFullDamage = plasmaProjectileStats.timeToFullDamage;
			((IPlasmaProjectileStatsComponent)this).startingRadiusScale = plasmaProjectileStats.startingRadiusScale;
			((IPlasmaProjectileStatsComponent)this).currentExplosionRadius = plasmaProjectileStats.currentExplosionRadius;
			base.SetGenericProjectileParamaters(weapon, launchPosition, direction, robotStartPos, isLocal);
		}
	}
}
