using Svelto.ECS;
using UnityEngine;

namespace Simulation.Hardware.Weapons.AeroFlak
{
	internal sealed class AeroflakProjectileMonoBehaviour : BaseProjectileMonoBehaviour, IAeroflakProjectileStatsComponent, IStackDamageComponent, ISplashDamageComponent, IEntitySourceComponent
	{
		public float ConeAngle = 30f;

		public int AdditionalHits = 6;

		private DispatchOnSet<bool> _stackableHit;

		float ISplashDamageComponent.damageRadius
		{
			get;
			set;
		}

		int IAeroflakProjectileStatsComponent.damageProximityHit
		{
			get;
			set;
		}

		float IAeroflakProjectileStatsComponent.damageRadius
		{
			get;
			set;
		}

		float IAeroflakProjectileStatsComponent.explosionRadius
		{
			get;
			set;
		}

		float IAeroflakProjectileStatsComponent.groundClearance
		{
			get;
			set;
		}

		public float coneAngle => ConeAngle;

		public int additionalHits => AdditionalHits;

		public int buffDamagePerStack
		{
			get;
			set;
		}

		public int buffMaxStacks
		{
			get;
			set;
		}

		public float buffStackExpireTime
		{
			get;
			set;
		}

		public int currentStackIndex
		{
			get;
			set;
		}

		public DispatchOnSet<bool> stackableHit => _stackableHit;

		internal override void SetGenericProjectileParamaters(WeaponShootingNode weapon, Vector3 launchPosition, Vector3 direction, Vector3 robotStartPos, bool isLocal)
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			IAeroflakProjectileStatsComponent aeroflakProjectileStats = (weapon as AeroflakWeaponShootingNode).aeroflakProjectileStats;
			((IAeroflakProjectileStatsComponent)this).damageProximityHit = aeroflakProjectileStats.damageProximityHit;
			((IAeroflakProjectileStatsComponent)this).damageRadius = aeroflakProjectileStats.damageRadius;
			((IAeroflakProjectileStatsComponent)this).explosionRadius = aeroflakProjectileStats.explosionRadius;
			((IAeroflakProjectileStatsComponent)this).groundClearance = aeroflakProjectileStats.groundClearance;
			IStackDamageStatsComponent stackDamageStats = (weapon as AeroflakWeaponShootingNode).stackDamageStats;
			((IStackDamageComponent)this).buffStackExpireTime = stackDamageStats.buffStackExpireTime;
			((IStackDamageComponent)this).buffMaxStacks = stackDamageStats.buffMaxStacks;
			((IStackDamageComponent)this).buffDamagePerStack = stackDamageStats.buffDamagePerStack;
			base.SetGenericProjectileParamaters(weapon, launchPosition, direction, robotStartPos, isLocal);
		}

		protected override void Awake()
		{
			base.Awake();
			_stackableHit = new DispatchOnSet<bool>(this.get_gameObject().GetInstanceID());
		}
	}
}
