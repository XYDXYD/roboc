using Svelto.ECS;
using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class BaseWeaponImplementor : MonoBehaviour, ICubePositionComponent, IGameObjectComponent, IItemDescriptorComponent, IWeaponCategoryComponent, IWeaponCrosshairTypeComponent, ICameraShakeComponent, IRobotShakeComponent, IWeaponActiveComponent, IZoomComponent, IWeaponFireCostComponent, IWeaponCooldownComponent, IFireOrderComponent, IPlayAfterEffectsComponent, IProjectileCreationComponent, IProjectileDamageStatsComponent, IVisibilityComponent, IVisibilityTracker, IMisfireComponent, IImplementor
	{
		public CrosshairType crosshairType;

		public TranslationCurve shootCameraTranslationCurves;

		public RotationCurve shootCameraRotationCurves;

		public float cameraShakeDuration = 0.215f;

		public float cameraShakeRotationMagnitude = 0.215f;

		public float cameraShakeTranslationMagnitude = 0.215f;

		public bool camShakeEnabled = true;

		public TranslationCurve shootRobotCurves;

		public float robotShakeDuration = 0.215f;

		public float robotShakeMagnitude = 0.215f;

		public bool robotShakeEnabled = true;

		public bool canZoom = true;

		public float zoomedFOV = 60f;

		private float _damageMultiplier = 1f;

		private float _damageBoost = 1f;

		private float _campaignDifficultyFactor = 1f;

		private DispatchOnSet<int> _applyShake;

		private Dispatcher<int> _nextElegibleWeaponToFire;

		private Dispatcher<IPlayAfterEffectsComponent, int> _applyRecoil;

		private Dispatcher<IPlayAfterEffectsComponent, int> _playMuzzleFlash;

		private Dispatcher<IPlayAfterEffectsComponent, int> _playFiringSound;

		private Dispatcher<IMisfireComponent, int> _weaponMisfired;

		private Dispatcher<IProjectileCreationComponent, ProjectileCreationParams> _createProjectile;

		private bool _isHidden;

		private bool _isInRange = true;

		private CameraShake _cameraShake;

		private Transform _transform;

		private GameObject _gameObject;

		private Byte3 _gridPos;

		private ItemDescriptor _itemDescriptor;

		private DispatchOnChange<bool> _onActiveChanged;

		GameObject IGameObjectComponent.gameObject
		{
			get
			{
				return _gameObject;
			}
		}

		Vector3 ICubePositionComponent.position
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return _transform.get_position();
			}
		}

		Byte3 ICubePositionComponent.gridPos
		{
			get
			{
				return _gridPos;
			}
		}

		ItemDescriptor IItemDescriptorComponent.itemDescriptor
		{
			get
			{
				return _itemDescriptor;
			}
		}

		ItemCategory IWeaponCategoryComponent.itemCategory
		{
			get
			{
				return _itemDescriptor.itemCategory;
			}
		}

		CrosshairType IWeaponCrosshairTypeComponent.crosshairType
		{
			get
			{
				return crosshairType;
			}
		}

		float IWeaponFireCostComponent.weaponFireCost
		{
			get;
			set;
		}

		float IWeaponCooldownComponent.weaponCooldown
		{
			get;
			set;
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
			get
			{
				return _damageMultiplier;
			}
			set
			{
				_damageMultiplier = value;
			}
		}

		float IProjectileDamageStatsComponent.damageBoost
		{
			get
			{
				return _damageBoost;
			}
			set
			{
				_damageBoost = value;
			}
		}

		float IProjectileDamageStatsComponent.campaignDifficultyFactor
		{
			get
			{
				return _campaignDifficultyFactor;
			}
			set
			{
				_campaignDifficultyFactor = value;
			}
		}

		float ICameraShakeComponent.cameraShakeDuration
		{
			get
			{
				return cameraShakeDuration;
			}
		}

		float ICameraShakeComponent.cameraShakeTranslationMagnitude
		{
			get
			{
				return cameraShakeTranslationMagnitude;
			}
		}

		float ICameraShakeComponent.cameraShakeRotationMagnitude
		{
			get
			{
				return cameraShakeRotationMagnitude;
			}
		}

		bool ICameraShakeComponent.camShakeEnabled
		{
			get
			{
				return camShakeEnabled;
			}
		}

		TranslationCurve ICameraShakeComponent.shootCameraTranslationCurves
		{
			get
			{
				return shootCameraTranslationCurves;
			}
		}

		RotationCurve ICameraShakeComponent.shootCameraRotationCurves
		{
			get
			{
				return shootCameraRotationCurves;
			}
		}

		DispatchOnSet<int> ICameraShakeComponent.applyShake
		{
			get
			{
				return _applyShake;
			}
		}

		float IRobotShakeComponent.robotShakeDuration
		{
			get
			{
				return robotShakeDuration;
			}
		}

		float IRobotShakeComponent.robotShakeMagnitude
		{
			get
			{
				return robotShakeMagnitude;
			}
		}

		bool IRobotShakeComponent.robotShakeEnabled
		{
			get
			{
				return robotShakeEnabled;
			}
		}

		TranslationCurve IRobotShakeComponent.shootRobotCurves
		{
			get
			{
				return shootRobotCurves;
			}
		}

		DispatchOnSet<int> IRobotShakeComponent.applyShake
		{
			get
			{
				return _applyShake;
			}
		}

		float IZoomComponent.zoomedFov
		{
			get
			{
				return zoomedFOV;
			}
		}

		bool IZoomComponent.canZoom
		{
			get
			{
				return canZoom;
			}
		}

		bool IZoomComponent.isZoomed
		{
			get;
			set;
		}

		Dispatcher<int> IFireOrderComponent.nextElegibleWeaponToFire
		{
			get
			{
				return _nextElegibleWeaponToFire;
			}
		}

		Dispatcher<IPlayAfterEffectsComponent, int> IPlayAfterEffectsComponent.applyRecoil
		{
			get
			{
				return _applyRecoil;
			}
		}

		Dispatcher<IPlayAfterEffectsComponent, int> IPlayAfterEffectsComponent.playMuzzleFlash
		{
			get
			{
				return _playMuzzleFlash;
			}
		}

		Dispatcher<IPlayAfterEffectsComponent, int> IPlayAfterEffectsComponent.playFiringSound
		{
			get
			{
				return _playFiringSound;
			}
		}

		Dispatcher<IProjectileCreationComponent, ProjectileCreationParams> IProjectileCreationComponent.createProjectile
		{
			get
			{
				return _createProjectile;
			}
		}

		Vector3 IProjectileCreationComponent.launchDirection
		{
			get;
			set;
		}

		bool IVisibilityTracker.isOffScreen
		{
			set
			{
				_isHidden = value;
			}
		}

		bool IVisibilityComponent.offScreen
		{
			get
			{
				return _isHidden;
			}
		}

		bool IVisibilityTracker.isInRange
		{
			set
			{
				_isInRange = value;
			}
		}

		bool IVisibilityComponent.inRange
		{
			get
			{
				return _isInRange;
			}
		}

		Dispatcher<IMisfireComponent, int> IMisfireComponent.weaponMisfired
		{
			get
			{
				return _weaponMisfired;
			}
		}

		float IMisfireComponent.coolDownPenalty
		{
			get;
			set;
		}

		int IMisfireComponent.misfireDebuffMaxStacks
		{
			get;
			set;
		}

		float IMisfireComponent.misfireDebuffDuration
		{
			get;
			set;
		}

		public ItemDescriptor itemDescriptor => _itemDescriptor;

		public bool active
		{
			get
			{
				return _onActiveChanged.get_value();
			}
			set
			{
				_onActiveChanged.set_value(value);
			}
		}

		public DispatchOnChange<bool> onActiveChanged => _onActiveChanged;

		public BaseWeaponImplementor()
			: this()
		{
		}

		private void OnValidate()
		{
			shootCameraTranslationCurves.ValidateInInspector();
			shootCameraRotationCurves.ValidateInInspector();
			shootRobotCurves.ValidateInInspector();
		}

		private void Awake()
		{
			_transform = this.get_transform();
			_gameObject = this.get_gameObject();
			if (this.GetComponent<ComponentTransformImplementor>() == null)
			{
				this.get_gameObject().AddComponent<ComponentTransformImplementor>();
			}
			if (this.GetComponent<HardwareWorkingOrderImplementor>() == null)
			{
				this.get_gameObject().AddComponent<HardwareWorkingOrderImplementor>();
			}
			_nextElegibleWeaponToFire = new Dispatcher<int>();
			_applyRecoil = new Dispatcher<IPlayAfterEffectsComponent, int>(this);
			_applyShake = new DispatchOnSet<int>(this.GetInstanceID());
			_playMuzzleFlash = new Dispatcher<IPlayAfterEffectsComponent, int>(this);
			_playFiringSound = new Dispatcher<IPlayAfterEffectsComponent, int>(this);
			_weaponMisfired = new Dispatcher<IMisfireComponent, int>(this);
			_createProjectile = new Dispatcher<IProjectileCreationComponent, ProjectileCreationParams>(this);
			_onActiveChanged = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
		}

		private void Start()
		{
			_cameraShake = Camera.get_main().GetComponent<CameraShake>();
		}

		internal void SetGridPos(Byte3 weaponGridPos)
		{
			_gridPos = weaponGridPos;
		}

		internal void SetDescriptor(ItemDescriptor itemDescriptor)
		{
			_itemDescriptor = itemDescriptor;
		}
	}
}
