using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal class IonDistorterMonoBehaviour : MonoBehaviour, IFireTimingComponent, IProjectileRangeComponent, IProjectileSpeedStatsComponent, IShootingComponent, IRecoilForceComponent, IHitSomethingComponent, IWeaponFiringAudioComponent, IWeaponNoFireAudioComponent, IProjectileEffectImpactSuccessfulComponent, IProjectileEffectImpactSelfComponent, IProjectileEffectImpactEnvironmentComponent, IProjectileEffectImpactProtoniumComponent, IProjectileEffectImpactFusionShieldComponent, IProjectileDeadEffectComponent, IWeaponAnimationComponent, IIonDistorterProjectileSettingsComponent, IProjectileEffectImpactEqualizerComponent
	{
		[SerializeField]
		private Animator animator;

		[SerializeField]
		private string _shootAnimationName;

		[SerializeField]
		private int numOfRaycasts;

		[SerializeField]
		private int numberOfSpreadCones;

		[SerializeField]
		private GameObject projectileDeadEffectPrefab;

		[SerializeField]
		private GameObject projectileDeadEffectPrefab_E;

		public float recoilForce = 10f;

		public GameObject impactSuccessfulPrefab;

		public GameObject impactSuccessfulPrefab_E;

		public GameObject impactSelfPrefab;

		public GameObject impactEnvironmentPrefab;

		public GameObject impactProtoniumPrefab;

		public GameObject impactProtoniumPrefab_E;

		public GameObject impactFusionShieldPrefab;

		public GameObject impactFusionShieldPrefab_E;

		public GameObject impactEqualizerPrefab;

		public GameObject impactEqualizerPrefab_E;

		public string firingAudio;

		public string noFireAudio;

		public string hitAudioEvent;

		public string enemyHasHitPlayerAudioEvent;

		public string enemyHasHitOtherAudioEvent;

		public string impactSuccessfulAudioEventHitMe;

		public string impactSelfAudioEvent;

		public string impactEnvironmentAudioEvent;

		public string impactFusionAudioEvent;

		private Dispatcher<IFireTimingComponent, ItemDescriptor> _fireTimingsLoaded;

		private Dispatcher<IShootingComponent, int> _shotIsReadyToFire;

		private Dispatcher<IShootingComponent, int> _shotIsGoingTobeFired;

		private Dispatcher<IShootingComponent> _shotCantBeFiredDueToLackOfMana;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitEnemy;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitAlly;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitEnemySplash;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitSelf;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitEnvironment;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitProtonium;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitFusionShield;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitSecondaryImpact;

		private Dispatcher<IHitSomethingComponent, HitInfo> _hitEqualizer;

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

		GameObject IProjectileEffectImpactSelfComponent.prefab
		{
			get
			{
				return impactSelfPrefab;
			}
		}

		string IProjectileEffectImpactSelfComponent.audioEvent
		{
			get
			{
				return impactSelfAudioEvent;
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

		GameObject IProjectileEffectImpactFusionShieldComponent.prefab
		{
			get
			{
				return impactFusionShieldPrefab;
			}
		}

		GameObject IProjectileEffectImpactFusionShieldComponent.prefab_E
		{
			get
			{
				return impactFusionShieldPrefab_E;
			}
		}

		string IProjectileEffectImpactFusionShieldComponent.audioEvent
		{
			get
			{
				return impactFusionAudioEvent;
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

		float IProjectileRangeComponent.maxRange
		{
			get;
			set;
		}

		float IProjectileSpeedStatsComponent.speed
		{
			get;
			set;
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
				return _shotIsGoingTobeFired;
			}
		}

		Dispatcher<IShootingComponent> IShootingComponent.shotCantBeFiredDueToLackOfMana
		{
			get
			{
				return _shotCantBeFiredDueToLackOfMana;
			}
		}

		float IRecoilForceComponent.recoilForce
		{
			get
			{
				return recoilForce;
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

		string IWeaponFiringAudioComponent.firingAudio
		{
			get
			{
				return firingAudio;
			}
		}

		string IWeaponNoFireAudioComponent.noFireAudio
		{
			get
			{
				return noFireAudio;
			}
		}

		Animator IWeaponAnimationComponent.animator
		{
			get
			{
				return animator;
			}
		}

		int IIonDistorterProjectileSettingsComponent.numOfRaycasts
		{
			get
			{
				return numOfRaycasts;
			}
		}

		int IIonDistorterProjectileSettingsComponent.numOfCircles
		{
			get
			{
				return numberOfSpreadCones;
			}
		}

		GameObject IProjectileDeadEffectComponent.prefab
		{
			get
			{
				return projectileDeadEffectPrefab;
			}
		}

		GameObject IProjectileDeadEffectComponent.prefab_E
		{
			get
			{
				return projectileDeadEffectPrefab_E;
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

		public string shootAnimationName => _shootAnimationName;

		public IonDistorterMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			_fireTimingsLoaded = new Dispatcher<IFireTimingComponent, ItemDescriptor>(this);
			_shotIsReadyToFire = new Dispatcher<IShootingComponent, int>(this);
			_shotIsGoingTobeFired = new Dispatcher<IShootingComponent, int>(this);
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
