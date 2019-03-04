using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class BaseProjectileMonoBehaviour : MonoBehaviour, IProjectileMovementStatsComponent, IProjectileDamageStatsComponent, IProjectileOwnerComponent, IProjectileTimeComponent, IProjectileRangeComponent, IProjectileAliveComponent, IWeaponOwnerPositionComponent, ITransformComponent, IWeaponDamageComponent, IPredictedProjectilePositionComponent, IProjectileStateComponent, IHitSomethingComponent, IWeaponIdComponent, IItemDescriptorComponent, IEntitySourceComponent
	{
		public float projectileLength = 1f;

		public int projectileHitDepth = 1;

		private Transform t;

		private Dispatcher<IProjectileAliveComponent, int> _resetProjectile;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitEnemy;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitAlly;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitEnemySplash;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitSelf;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitEnvironment;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitProtonium;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitFusionShield;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitSecondaryImpact;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitEqualizer;

		private Dispatcher<IWeaponDamageComponent, int> _weaponDamage;

		private ItemDescriptor _subCategory;

		Dispatcher<IHitSomethingComponent, HitInfo> IHitSomethingComponent.hitEnemy
		{
			get
			{
				return _hitEnemy;
			}
		}

		Dispatcher<IHitSomethingComponent, HitInfo> IHitSomethingComponent.hitAlly
		{
			get
			{
				return _hitAlly;
			}
		}

		Dispatcher<IHitSomethingComponent, HitInfo> IHitSomethingComponent.hitEnemySplash
		{
			get
			{
				return _hitEnemySplash;
			}
		}

		Dispatcher<IHitSomethingComponent, HitInfo> IHitSomethingComponent.hitSelf
		{
			get
			{
				return _hitSelf;
			}
		}

		Dispatcher<IHitSomethingComponent, HitInfo> IHitSomethingComponent.hitEnvironment
		{
			get
			{
				return _hitEnvironment;
			}
		}

		Dispatcher<IHitSomethingComponent, HitInfo> IHitSomethingComponent.hitProtonium
		{
			get
			{
				return _hitProtonium;
			}
		}

		Dispatcher<IHitSomethingComponent, HitInfo> IHitSomethingComponent.hitFusionShield
		{
			get
			{
				return _hitFusionShield;
			}
		}

		Dispatcher<IHitSomethingComponent, HitInfo> IHitSomethingComponent.hitSecondaryImpact
		{
			get
			{
				return _hitSecondaryImpact;
			}
		}

		Dispatcher<IHitSomethingComponent, HitInfo> IHitSomethingComponent.hitEqualizer
		{
			get
			{
				return _hitEqualizer;
			}
		}

		ItemDescriptor IItemDescriptorComponent.itemDescriptor
		{
			get
			{
				return _subCategory;
			}
		}

		Vector3 IPredictedProjectilePositionComponent.currentPos
		{
			get;
			set;
		}

		Vector3 IPredictedProjectilePositionComponent.nextPos
		{
			get;
			set;
		}

		bool IProjectileAliveComponent.justFired
		{
			get;
			set;
		}

		bool IProjectileAliveComponent.active
		{
			get;
			set;
		}

		Dispatcher<IProjectileAliveComponent, int> IProjectileAliveComponent.resetProjectile
		{
			get
			{
				return _resetProjectile;
			}
		}

		int IProjectileDamageStatsComponent.damage
		{
			get;
			set;
		}

		float IProjectileDamageStatsComponent.protoniumDamageScale
		{
			get;
			set;
		}

		float IProjectileDamageStatsComponent.damageBuff
		{
			get;
			set;
		}

		float IProjectileDamageStatsComponent.damageMultiplier
		{
			get;
			set;
		}

		float IProjectileDamageStatsComponent.damageBoost
		{
			get;
			set;
		}

		float IProjectileDamageStatsComponent.campaignDifficultyFactor
		{
			get;
			set;
		}

		Vector3 IProjectileMovementStatsComponent.startDirection
		{
			get;
			set;
		}

		Vector3 IProjectileMovementStatsComponent.startPosition
		{
			get;
			set;
		}

		Vector3 IProjectileMovementStatsComponent.buildPosition
		{
			get;
			set;
		}

		Vector3 IProjectileMovementStatsComponent.startVelocity
		{
			get;
			set;
		}

		Vector3 IProjectileMovementStatsComponent.robotStartPosition
		{
			get;
			set;
		}

		float IProjectileMovementStatsComponent.speed
		{
			get;
			set;
		}

		Byte3 IProjectileOwnerComponent.weaponGridPos
		{
			get;
			set;
		}

		int IProjectileOwnerComponent.ownerId
		{
			get;
			set;
		}

		int IProjectileOwnerComponent.machineId
		{
			get;
			set;
		}

		int IProjectileOwnerComponent.ownerTeam
		{
			get;
			set;
		}

		bool IProjectileOwnerComponent.isEnemy
		{
			get;
			set;
		}

		bool IProjectileOwnerComponent.isAi
		{
			get;
			set;
		}

		bool IProjectileOwnerComponent.ownedByMe
		{
			get;
			set;
		}

		float IProjectileRangeComponent.maxRange
		{
			get;
			set;
		}

		bool IProjectileStateComponent.disabled
		{
			get;
			set;
		}

		float IProjectileTimeComponent.maxTime
		{
			get;
			set;
		}

		float IProjectileTimeComponent.startTime
		{
			get;
			set;
		}

		Transform ITransformComponent.T
		{
			get
			{
				return t;
			}
		}

		int IWeaponDamageComponent.numHits
		{
			get;
			set;
		}

		int IWeaponDamageComponent.hitDepth
		{
			get
			{
				return projectileHitDepth;
			}
		}

		HitResult[] IWeaponDamageComponent.hitResults
		{
			get;
			set;
		}

		Dispatcher<IWeaponDamageComponent, int> IWeaponDamageComponent.weaponDamage
		{
			get
			{
				return _weaponDamage;
			}
		}

		int IWeaponIdComponent.weaponId
		{
			get;
			set;
		}

		Byte3 IWeaponOwnerPositionComponent.gridPosition
		{
			get;
			set;
		}

		public bool isLocal
		{
			get;
			private set;
		}

		public BaseProjectileMonoBehaviour()
			: this()
		{
		}

		protected virtual void Awake()
		{
			_resetProjectile = new Dispatcher<IProjectileAliveComponent, int>(this);
			_hitEnemy = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitAlly = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitEnemySplash = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitSelf = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitEnvironment = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitProtonium = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitFusionShield = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitSecondaryImpact = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitEqualizer = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_weaponDamage = new Dispatcher<IWeaponDamageComponent, int>(this);
			t = this.get_transform();
		}

		internal virtual void SetGenericProjectileParamaters(WeaponShootingNode weapon, Vector3 launchPosition, Vector3 direction, Vector3 robotStartPos, bool isLocal)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			((IWeaponIdComponent)this).weaponId = weapon.get_ID();
			((IProjectileMovementStatsComponent)this).startDirection = direction;
			((IProjectileMovementStatsComponent)this).startPosition = launchPosition;
			((IProjectileMovementStatsComponent)this).robotStartPosition = robotStartPos;
			((IProjectileMovementStatsComponent)this).buildPosition = launchPosition;
			((IProjectileMovementStatsComponent)this).speed = weapon.projectileSpeedStats.speed;
			((IProjectileMovementStatsComponent)this).startVelocity = direction * ((IProjectileMovementStatsComponent)this).speed;
			IProjectileDamageStatsComponent projectileDamageStats = weapon.projectileDamageStats;
			((IProjectileDamageStatsComponent)this).damage = projectileDamageStats.damage;
			((IProjectileDamageStatsComponent)this).protoniumDamageScale = projectileDamageStats.protoniumDamageScale;
			((IProjectileDamageStatsComponent)this).damageBuff = projectileDamageStats.damageBuff;
			((IProjectileDamageStatsComponent)this).damageMultiplier = projectileDamageStats.damageMultiplier;
			((IProjectileDamageStatsComponent)this).damageBoost = projectileDamageStats.damageBoost;
			((IProjectileDamageStatsComponent)this).campaignDifficultyFactor = projectileDamageStats.campaignDifficultyFactor;
			((IProjectileRangeComponent)this).maxRange = weapon.projectileRangeStats.maxRange;
			((IProjectileTimeComponent)this).maxTime = 60f;
			((IProjectileTimeComponent)this).startTime = Time.get_timeSinceLevelLoad();
			IHardwareOwnerComponent weaponOwner = weapon.weaponOwner;
			((IProjectileOwnerComponent)this).weaponGridPos = weapon.cubePositionComponent.gridPos;
			((IProjectileOwnerComponent)this).ownerId = weaponOwner.ownerId;
			((IProjectileOwnerComponent)this).machineId = weaponOwner.machineId;
			((IProjectileOwnerComponent)this).ownerTeam = weaponOwner.ownerTeam;
			((IProjectileOwnerComponent)this).isEnemy = weaponOwner.isEnemy;
			((IProjectileOwnerComponent)this).ownedByMe = weaponOwner.ownedByMe;
			((IProjectileOwnerComponent)this).isAi = weaponOwner.ownedByAi;
			((IProjectileAliveComponent)this).justFired = true;
			((IProjectileAliveComponent)this).active = true;
			((IProjectileStateComponent)this).disabled = false;
			((IWeaponOwnerPositionComponent)this).gridPosition = weapon.cubePositionComponent.gridPos;
			IHitSomethingComponent hitSomethingComponent = weapon.hitSomethingComponent;
			_hitEnemy = hitSomethingComponent.hitEnemy;
			_hitAlly = hitSomethingComponent.hitAlly;
			_hitEnemySplash = hitSomethingComponent.hitEnemySplash;
			_hitSelf = hitSomethingComponent.hitSelf;
			_hitEnvironment = hitSomethingComponent.hitEnvironment;
			_hitProtonium = hitSomethingComponent.hitProtonium;
			_hitFusionShield = hitSomethingComponent.hitFusionShield;
			_hitSecondaryImpact = hitSomethingComponent.hitSecondaryImpact;
			_hitEqualizer = hitSomethingComponent.hitEqualizer;
			_subCategory = weapon.itemDescriptorComponent.itemDescriptor;
			this.get_transform().set_position(launchPosition);
			this.get_transform().set_rotation(Quaternion.LookRotation(direction));
			this.isLocal = isLocal;
		}
	}
}
