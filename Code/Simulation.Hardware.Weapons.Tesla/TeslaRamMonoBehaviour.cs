using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal sealed class TeslaRamMonoBehaviour : MonoBehaviour, ICollisionComponent, IProjectileEffectImpactEnvironmentComponent, IProjectileEffectImpactSuccessfulComponent, IProjectileEffectImpactProtoniumComponent, ITeslaDamageStats, IAudioOnEnabledComponent, ITeslaEffectComponent, IFireTimingComponent, IHitSomethingComponent, IShootingComponent, IWeaponMovementComponent, ITeslaTargetComponent, IAudioOnDisabledComponent, ITeslaBladesComponent, IProjectileEffectImpactEqualizerComponent, IFrontPositionComponent, IWeaponAccuracyModifierComponent
	{
		public Animation teslaAnimation;

		public GameObject teslaBladesParent;

		public GameObject impactSuccessfulPrefab;

		public GameObject impactSuccessfulPrefab_E;

		public GameObject impactEnvironmentPrefab;

		public GameObject impactProtoniumPrefab;

		public GameObject impactProtoniumPrefab_E;

		public GameObject impactEqualizerPrefab;

		public GameObject impactEqualizerPrefab_E;

		public Transform frontTransform;

		public string hitAudioEvent;

		public string enemyHasHitPlayerAudioEvent;

		public string enemyHasHitOtherAudioEvent;

		public string audioOnEnabled;

		public string audioOnDisabled;

		public string audioLoop;

		public string impactEnvironmentAudioEvent;

		private Vector3 _initialPosition;

		private Quaternion _initialRotation;

		private FasterList<Collider> _targetColliders = new FasterList<Collider>();

		private Dispatcher<ITeslaEffectComponent, int> _triggerEffectExit;

		private DispatchOnSet<Collider> _onTriggerEnter;

		private DispatchOnSet<Collider> _onTriggerExit;

		private Dispatcher<IFireTimingComponent, ItemDescriptor> _fireTimingsLoaded;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitEnemy;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitAlly;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitEnemySplash;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitSelf;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitEnvironment;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitProtonium;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitFusionShield;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitSecondaryImpact;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitEqualizer;

		private Dispatcher<IShootingComponent, int> _shotIsReadyToFire;

		private Dispatcher<IShootingComponent, int> _shotIsGoingToBeFired;

		private Dispatcher<IShootingComponent> _shotCantBeFiredDueToLackOfMana;

		DispatchOnSet<Collider> ICollisionComponent.onTriggerEnter
		{
			get
			{
				return _onTriggerEnter;
			}
		}

		DispatchOnSet<Collider> ICollisionComponent.onTriggerExit
		{
			get
			{
				return _onTriggerExit;
			}
		}

		GameObject IProjectileEffectImpactSuccessfulComponent.prefab
		{
			get
			{
				return impactSuccessfulPrefab;
			}
		}

		GameObject IProjectileEffectImpactSuccessfulComponent.prefab_E
		{
			get
			{
				return impactSuccessfulPrefab_E;
			}
		}

		string IProjectileEffectImpactSuccessfulComponent.audioEvent
		{
			get
			{
				return hitAudioEvent;
			}
		}

		string IProjectileEffectImpactSuccessfulComponent.audioEventHitMe
		{
			get
			{
				return enemyHasHitPlayerAudioEvent;
			}
		}

		string IProjectileEffectImpactSuccessfulComponent.audioEventEnemyHitOther
		{
			get
			{
				return enemyHasHitOtherAudioEvent;
			}
		}

		GameObject IProjectileEffectImpactEnvironmentComponent.prefab
		{
			get
			{
				return impactEnvironmentPrefab;
			}
		}

		string IProjectileEffectImpactEnvironmentComponent.audioEvent
		{
			get
			{
				return impactEnvironmentAudioEvent;
			}
		}

		GameObject IProjectileEffectImpactProtoniumComponent.prefab
		{
			get
			{
				return impactProtoniumPrefab;
			}
		}

		GameObject IProjectileEffectImpactProtoniumComponent.prefab_E
		{
			get
			{
				return impactProtoniumPrefab_E;
			}
		}

		GameObject IProjectileEffectImpactEqualizerComponent.prefab
		{
			get
			{
				return impactEqualizerPrefab;
			}
		}

		GameObject IProjectileEffectImpactEqualizerComponent.prefab_E
		{
			get
			{
				return impactEqualizerPrefab_E;
			}
		}

		float ITeslaDamageStats.teslaDamage
		{
			get;
			set;
		}

		float ITeslaDamageStats.teslaCharges
		{
			get;
			set;
		}

		float IFireTimingComponent.delayBetweenShots
		{
			get;
			set;
		}

		float[] IFireTimingComponent.groupFirePeriod
		{
			get;
			set;
		}

		Dispatcher<IFireTimingComponent, ItemDescriptor> IFireTimingComponent.timingsLoaded
		{
			get
			{
				return _fireTimingsLoaded;
			}
		}

		string IAudioOnEnabledComponent.audioOnEnabled
		{
			get
			{
				return audioOnEnabled;
			}
		}

		string IAudioOnDisabledComponent.audioOnDisabled
		{
			get
			{
				return audioOnDisabled;
			}
		}

		bool ITeslaEffectComponent.isOpen
		{
			get;
			set;
		}

		string ITeslaEffectComponent.audioLoop
		{
			get
			{
				return audioLoop;
			}
		}

		Animation ITeslaEffectComponent.animation
		{
			get
			{
				return teslaAnimation;
			}
		}

		Dispatcher<ITeslaEffectComponent, int> ITeslaEffectComponent.triggerExit
		{
			get
			{
				return _triggerEffectExit;
			}
		}

		bool ITeslaTargetComponent.hasTarget
		{
			get;
			set;
		}

		int ITeslaTargetComponent.playerId
		{
			get;
			set;
		}

		int ITeslaTargetComponent.machineId
		{
			get;
			set;
		}

		TargetType ITeslaTargetComponent.targetType
		{
			get;
			set;
		}

		Transform ITeslaTargetComponent.hitObjectTransform
		{
			get;
			set;
		}

		Vector3 ITeslaTargetComponent.hitPoint
		{
			get;
			set;
		}

		FasterList<Collider> ITeslaTargetComponent.targetColliders
		{
			get
			{
				return _targetColliders;
			}
		}

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

		bool IShootingComponent.justFired
		{
			get;
			set;
		}

		float IShootingComponent.lastFireTime
		{
			get;
			set;
		}

		Dispatcher<IShootingComponent, int> IShootingComponent.shotIsReadyToFire
		{
			get
			{
				return _shotIsReadyToFire;
			}
		}

		Dispatcher<IShootingComponent, int> IShootingComponent.shotIsGoingToBeFired
		{
			get
			{
				return _shotIsGoingToBeFired;
			}
		}

		Dispatcher<IShootingComponent> IShootingComponent.shotCantBeFiredDueToLackOfMana
		{
			get
			{
				return _shotCantBeFiredDueToLackOfMana;
			}
		}

		Vector3 IFrontPositionComponent.position
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return frontTransform.get_position();
			}
		}

		Vector3 IWeaponMovementComponent.velocity
		{
			get;
			set;
		}

		Vector3 IWeaponMovementComponent.rotationVelocity
		{
			get;
			set;
		}

		Vector3 IWeaponMovementComponent.lastPosition
		{
			get;
			set;
		}

		Vector3 IWeaponMovementComponent.lastRotation
		{
			get;
			set;
		}

		public GameObject teslaBladesParentGameObject => teslaBladesParent;

		public float totalAccuracy
		{
			get;
			set;
		}

		public float crosshairAccuracyModifier
		{
			get;
			set;
		}

		public float repeatFiringModifier
		{
			get;
			set;
		}

		public float movementAccuracyModifier
		{
			get;
			set;
		}

		public TeslaRamMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			int instanceID = this.get_gameObject().GetInstanceID();
			_onTriggerEnter = new DispatchOnSet<Collider>(instanceID);
			_onTriggerExit = new DispatchOnSet<Collider>(instanceID);
			_triggerEffectExit = new Dispatcher<ITeslaEffectComponent, int>(this);
			_fireTimingsLoaded = new Dispatcher<IFireTimingComponent, ItemDescriptor>(this);
			_hitEnemy = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitAlly = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitEnemySplash = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitSelf = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitEnvironment = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitProtonium = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitFusionShield = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitSecondaryImpact = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitEqualizer = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_shotIsReadyToFire = new Dispatcher<IShootingComponent, int>(this);
			_shotIsGoingToBeFired = new Dispatcher<IShootingComponent, int>(this);
			_shotCantBeFiredDueToLackOfMana = new Dispatcher<IShootingComponent>();
		}

		public void OnTriggerEnterEvent(ref Collider c)
		{
			_onTriggerEnter.set_value(c);
		}

		public void OnTriggerExitEvent(ref Collider c)
		{
			_onTriggerExit.set_value(c);
		}
	}
}
