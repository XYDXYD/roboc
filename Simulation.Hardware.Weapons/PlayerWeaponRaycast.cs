using Svelto.Context;
using Svelto.IoC;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class PlayerWeaponRaycast : WeaponRaycast, IInitialize, IWaitForFrameworkDestruction
	{
		private const float OFFSET_TO_AVOID_NEAR_COLLISIONS = 0.5f;

		private static float _raycastLength = 1f;

		private Vector3 _screenPoint;

		private Vector3 _direction;

		private Ray _ray;

		private Camera _mainCamera;

		private Transform _cameraTransform;

		private ItemCategory _currentItemType;

		[Inject]
		internal MachineRootContainer machineRootContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			get;
			private set;
		}

		[Inject]
		internal RigidbodyDataContainer rigidbodyDataContainer
		{
			private get;
			set;
		}

		[Inject]
		internal NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		[Inject]
		internal SwitchWeaponObserver weaponObserver
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			weaponObserver.SwitchWeaponsEvent += WeaponTypeChanged;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			weaponObserver.SwitchWeaponsEvent -= WeaponTypeChanged;
		}

		public override void SetMaxRange(float range)
		{
			_raycastLength = Mathf.Max(_raycastLength, range * 1.5f);
		}

		public override Vector3 GetHitFreeForward()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _direction;
		}

		private void Start()
		{
			_mainCamera = Camera.get_main();
			_cameraTransform = _mainCamera.get_transform();
		}

		private void Update()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			_screenPoint = GetScreenCentreScreenSpace();
			_ray = _mainCamera.ScreenPointToRay(_screenPoint);
			_direction = _cameraTransform.get_forward();
			_ray.set_origin(_ray.get_origin() + _direction * 0.5f);
			UpdateWeaponRaycast();
		}

		private void UpdateWeaponRaycast()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			WeaponRaycastUtility.Ray ray = default(WeaponRaycastUtility.Ray);
			ray.startPosition = _ray.get_origin();
			ray.direction = _direction;
			ray.range = _raycastLength;
			HitResult[] array = new HitResult[1];
			WeaponRaycastUtility.Parameters parameters = default(WeaponRaycastUtility.Parameters);
			parameters.machineRootContainer = machineRootContainer;
			parameters.playerTeamsContainer = playerTeamsContainer;
			parameters.playerMachinesContainer = playerMachinesContainer;
			parameters.machineManager = machineManager;
			parameters.shooterId = playerTeamsContainer.localPlayerId;
			parameters.isShooterAi = false;
			bool ignoreTeamMates = _currentItemType != ItemCategory.Nano;
			WeaponRaycastUtility.RaycastWeaponAim(ref ray, ref parameters, array, ignoreTeamMates);
			HitResult hitResult = array[0];
			base.aimPoint = hitResult.hitPoint;
			base.targetPoint = hitResult.hitPoint;
			if (LayerToTargetType.IsTargetDestructible(hitResult.targetType))
			{
				base.targetRigidbody = rigidbodyDataContainer.GetRigidBodyData(hitResult.targetType, hitResult.hitTargetMachineId);
			}
		}

		private Vector3 GetScreenCentreScreenSpace()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3((float)Screen.get_width() * 0.5f, (float)Screen.get_height() * 0.5f, 0f);
		}

		private void WeaponTypeChanged(int machineId, ItemDescriptor itemDescriptor)
		{
			_currentItemType = itemDescriptor.itemCategory;
		}
	}
}
