using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Nano
{
	internal sealed class NanoWeaponMonoBehaviour : MonoBehaviour, IFireTimingComponent, IProjectileRangeComponent, IProjectileSpeedStatsComponent, ISplashDamageComponent, IHomingProjectileStatsComponent, IProjectileEffectImpactSuccessfulComponent, IProjectileEffectImpactEnvironmentComponent, IShootingComponent, IHitSomethingComponent, IWeaponFiringAudioComponent, IWeaponNoFireAudioComponent, ILockOnTargetingParametersComponent, IProjectileEffectImpactMissComponent, IRecoilForceComponent
	{
		public Dispatcher<IFireTimingComponent, ItemDescriptor> _fireTimingsLoaded;

		[SerializeField]
		private string _firingAudio;

		[SerializeField]
		private string _noFireAudio;

		[SerializeField]
		private float lockConeDotLowerLimit = 0.996f;

		[SerializeField]
		private bool looseLock;

		[SerializeField]
		private bool notifyTargetAboutLock;

		public Dispatcher<IShootingComponent, int> _shotIsReadyToFire;

		public Dispatcher<IShootingComponent, int> _shotIsGoingToBeFired;

		public Dispatcher<IShootingComponent> _shotCantBeFiredDueToLackOfMana;

		public Dispatcher<IHitSomethingComponent, HitInfo> _hitEnemy;

		public Dispatcher<IHitSomethingComponent, HitInfo> _hitAlly;

		public Dispatcher<IHitSomethingComponent, HitInfo> _hitEnemySplash;

		public Dispatcher<IHitSomethingComponent, HitInfo> _hitSelf;

		public Dispatcher<IHitSomethingComponent, HitInfo> _hitEnvironment;

		public Dispatcher<IHitSomethingComponent, HitInfo> _hitProtonium;

		public Dispatcher<IHitSomethingComponent, HitInfo> _hitFusionShield;

		public Dispatcher<IHitSomethingComponent, HitInfo> _hitSecondaryImpact;

		public Dispatcher<IHitSomethingComponent, HitInfo> _hitEqualizer;

		[SerializeField]
		private GameObject impactSuccessfulPrefab;

		[SerializeField]
		private GameObject impactSuccessfulPrefab_E;

		[SerializeField]
		private string hitAudioEvent;

		[SerializeField]
		private string enemyHasHitPlayerAudioEvent;

		[SerializeField]
		private string enemyHasHitOtherAudioEvent;

		[SerializeField]
		private GameObject impactMissPrefab;

		[SerializeField]
		private string missAudioEvent;

		[SerializeField]
		private GameObject impactEnvironmentPrefab;

		[SerializeField]
		private string impactEnvironmentAudioEvent;

		private string _enemyPlayerFiringAudio;

		private string _enemyPlayerFiringMeAudio;

		private string _stopFiringAudio;

		GameObject IProjectileEffectImpactSuccessfulComponent.prefab
		{
			get
			{
				return impactSuccessfulPrefab;
			}
		}

		string IProjectileEffectImpactSuccessfulComponent.audioEvent
		{
			get
			{
				return hitAudioEvent;
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

		GameObject IProjectileEffectImpactMissComponent.prefab
		{
			get
			{
				return impactMissPrefab;
			}
		}

		string IProjectileEffectImpactMissComponent.audioEvent
		{
			get
			{
				return missAudioEvent;
			}
		}

		string IWeaponFiringAudioComponent.enemyPlayerFiringAudio
		{
			get
			{
				return _enemyPlayerFiringAudio;
			}
		}

		string IWeaponFiringAudioComponent.enemyPlayerFiringMeAudio
		{
			get
			{
				return _enemyPlayerFiringMeAudio;
			}
		}

		string IWeaponFiringAudioComponent.stopFiringAudio
		{
			get
			{
				return _stopFiringAudio;
			}
		}

		public float delayBetweenShots
		{
			get;
			set;
		}

		public float[] groupFirePeriod
		{
			get;
			set;
		}

		public Dispatcher<IFireTimingComponent, ItemDescriptor> timingsLoaded => _fireTimingsLoaded;

		public float maxRange
		{
			get;
			set;
		}

		public float speed
		{
			get;
			set;
		}

		public float damageRadius
		{
			get;
			set;
		}

		public float coneAngle
		{
			get;
			set;
		}

		public int additionalHits
		{
			get;
			set;
		}

		public Vector3 angularVelocity
		{
			get;
			set;
		}

		public float maxRotationAccelerationRad
		{
			get;
			set;
		}

		public float maxRotationSpeedRad
		{
			get;
			set;
		}

		public float initialRotationSpeedRad
		{
			get;
			set;
		}

		public string firingAudio => _firingAudio;

		public string noFireAudio => _noFireAudio;

		public float lockTime
		{
			get;
			set;
		}

		public float fullLockReleaseTime
		{
			get;
			set;
		}

		public float changeLockTime
		{
			get;
			set;
		}

		public float lockOnConeDot => lockConeDotLowerLimit;

		public bool isLooseLock => looseLock;

		public bool notifyTargetOfLock => notifyTargetAboutLock;

		public bool justFired
		{
			get;
			set;
		}

		public float lastFireTime
		{
			get;
			set;
		}

		public Dispatcher<IShootingComponent, int> shotIsReadyToFire => _shotIsReadyToFire;

		public Dispatcher<IShootingComponent, int> shotIsGoingToBeFired => _shotIsGoingToBeFired;

		public Dispatcher<IShootingComponent> shotCantBeFiredDueToLackOfMana => _shotCantBeFiredDueToLackOfMana;

		public Dispatcher<IHitSomethingComponent, HitInfo> hitEnemy => _hitEnemy;

		public Dispatcher<IHitSomethingComponent, HitInfo> hitAlly => _hitAlly;

		public Dispatcher<IHitSomethingComponent, HitInfo> hitEnemySplash => _hitEnemySplash;

		public Dispatcher<IHitSomethingComponent, HitInfo> hitSelf => _hitSelf;

		public Dispatcher<IHitSomethingComponent, HitInfo> hitEnvironment => _hitEnvironment;

		public Dispatcher<IHitSomethingComponent, HitInfo> hitProtonium => _hitProtonium;

		public Dispatcher<IHitSomethingComponent, HitInfo> hitFusionShield => _hitFusionShield;

		public Dispatcher<IHitSomethingComponent, HitInfo> hitSecondaryImpact => _hitSecondaryImpact;

		public Dispatcher<IHitSomethingComponent, HitInfo> hitEqualizer => _hitEqualizer;

		public GameObject prefab_E => impactSuccessfulPrefab_E;

		public string audioEventHitMe => enemyHasHitPlayerAudioEvent;

		public string audioEventEnemyHitOther => enemyHasHitOtherAudioEvent;

		public float recoilForce => 0f;

		public NanoWeaponMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			_fireTimingsLoaded = new Dispatcher<IFireTimingComponent, ItemDescriptor>(this);
			_shotIsReadyToFire = new Dispatcher<IShootingComponent, int>(this);
			_shotIsGoingToBeFired = new Dispatcher<IShootingComponent, int>(this);
			_shotCantBeFiredDueToLackOfMana = new Dispatcher<IShootingComponent>();
			_hitEnemy = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitAlly = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitEnemySplash = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitSelf = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitEnvironment = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitProtonium = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitFusionShield = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_hitSecondaryImpact = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
			_enemyPlayerFiringAudio = FastConcatUtility.FastConcat<string>(firingAudio, "_Enemy");
			_enemyPlayerFiringMeAudio = FastConcatUtility.FastConcat<string>(firingAudio, "_Enemy_Target_Me");
			_stopFiringAudio = FastConcatUtility.FastConcat<string>(firingAudio, "_Stop");
			_hitEqualizer = new Dispatcher<IHitSomethingComponent, HitInfo>(this);
		}
	}
}
