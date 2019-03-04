using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.RailGun
{
	internal sealed class RailAimGraphicsEngine : MultiEntityViewsEngine<WeaponWithLaserPointerNode, WeaponWithoutLaserPointerNode>, IInitialize, ITickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private IRailStateGraphicsData _data;

		private Dictionary<int, MachinePointerData> _infoPerMachine = new Dictionary<int, MachinePointerData>();

		private Dictionary<int, WeaponWithLaserPointerNode> _activeWeaponPerMachine = new Dictionary<int, WeaponWithLaserPointerNode>();

		private Transform _mainCamera;

		private GameObject _currentPrefab;

		private readonly Func<GameObject> _onLaserAllocation;

		[Inject]
		public PlayerMachinesContainer playerMachines
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeams
		{
			private get;
			set;
		}

		[Inject]
		public MachineRootContainer machineRoots
		{
			private get;
			set;
		}

		[Inject]
		public NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		[Inject]
		public GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public RailAimGraphicsEngine()
		{
			_onLaserAllocation = OnEffectAllocation;
		}

		public void Ready()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			GameObject val = Resources.Load<GameObject>("RailStateGraphicsData");
			_data = val.GetComponent<RailStateGraphicsData>();
			PreallocateLasers();
		}

		private GameObject OnEffectAllocation()
		{
			return gameObjectPool.AddGameObjectWithoutRecycle(_currentPrefab);
		}

		private void PreallocateLasers()
		{
			_currentPrefab = _data.targetingLaserPrefab;
			gameObjectPool.Preallocate(_currentPrefab.get_name(), 1, _onLaserAllocation);
			_currentPrefab = _data.targetingLaserPrefabAlly;
			gameObjectPool.Preallocate(_currentPrefab.get_name(), 3, _onLaserAllocation);
			_currentPrefab = _data.targetingLaserPrefabEnemy;
			gameObjectPool.Preallocate(_currentPrefab.get_name(), 10, _onLaserAllocation);
		}

		protected override void Add(WeaponWithLaserPointerNode obj)
		{
			obj.fireOrderComponent.nextElegibleWeaponToFire.subscribers += EnableLaserOnSwitchingToAWeaponWithLaserPointer;
			InstantiateLaserPointer(obj.ownerComponent);
		}

		protected override void Remove(WeaponWithLaserPointerNode obj)
		{
			obj.fireOrderComponent.nextElegibleWeaponToFire.subscribers -= EnableLaserOnSwitchingToAWeaponWithLaserPointer;
			int machineId = obj.ownerComponent.machineId;
			_activeWeaponPerMachine.Remove(machineId);
			RecyclePointerLaser(machineId);
		}

		protected override void Add(WeaponWithoutLaserPointerNode obj)
		{
			obj.fireOrderComponent.nextElegibleWeaponToFire.subscribers += DisableLaserOnSwitchingToAWeaponWithoutLaserPointer;
		}

		protected override void Remove(WeaponWithoutLaserPointerNode obj)
		{
			obj.fireOrderComponent.nextElegibleWeaponToFire.subscribers -= DisableLaserOnSwitchingToAWeaponWithoutLaserPointer;
		}

		public void Tick(float deltaTime)
		{
			if (_mainCamera == null)
			{
				_mainCamera = Camera.get_main().get_transform();
			}
			Dictionary<int, WeaponWithLaserPointerNode>.Enumerator enumerator = _activeWeaponPerMachine.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int key = enumerator.Current.Key;
				WeaponWithLaserPointerNode value = enumerator.Current.Value;
				UpdateLaserTarget(key, value);
			}
		}

		private void InstantiateLaserPointer(IHardwareOwnerComponent ownerComponent)
		{
			int machineId = ownerComponent.machineId;
			if (!_infoPerMachine.ContainsKey(machineId))
			{
				if (ownerComponent.ownedByMe)
				{
					_currentPrefab = _data.targetingLaserPrefab;
					GameObject val = gameObjectPool.Use(_currentPrefab.get_name(), _onLaserAllocation);
					_infoPerMachine[machineId] = new MachinePointerData
					{
						laser = val.get_transform()
					};
				}
				else
				{
					_currentPrefab = ((!ownerComponent.isEnemy) ? _data.targetingLaserPrefabAlly : _data.targetingLaserPrefabEnemy);
					GameObject val2 = gameObjectPool.Use(_currentPrefab.get_name(), _onLaserAllocation);
					_infoPerMachine[machineId] = new MachinePointerData
					{
						laser = val2.get_transform(),
						material = val2.GetComponentInChildren<Renderer>().get_material()
					};
				}
			}
		}

		private void RecyclePointerLaser(int machineId)
		{
			if (_infoPerMachine.TryGetValue(machineId, out MachinePointerData value))
			{
				GameObject gameObject = value.laser.get_gameObject();
				gameObject.SetActive(false);
				gameObjectPool.Recycle(gameObject, gameObject.get_name());
				_infoPerMachine.Remove(machineId);
			}
		}

		private void EnableLaserOnSwitchingToAWeaponWithLaserPointer(int senderId)
		{
			WeaponWithLaserPointerNode weaponWithLaserPointerNode = entityViewsDB.QueryEntityView<WeaponWithLaserPointerNode>(senderId);
			int machineId = weaponWithLaserPointerNode.ownerComponent.machineId;
			AttachLaserToWeapon(machineId, weaponWithLaserPointerNode);
		}

		private void DisableLaserOnSwitchingToAWeaponWithoutLaserPointer(int senderId)
		{
			IHardwareOwnerComponent ownerComponent = entityViewsDB.QueryEntityView<NextWeaponToFireNode>(senderId).ownerComponent;
			int machineId = ownerComponent.machineId;
			HideLaser(machineId);
		}

		private void AttachLaserToWeapon(int machineId, WeaponWithLaserPointerNode weapon)
		{
			_activeWeaponPerMachine[machineId] = weapon;
			UpdateLaserTarget(machineId, weapon);
			MachinePointerData machinePointerData = _infoPerMachine[machineId];
			Transform laser = machinePointerData.laser;
			if (laser != null)
			{
				laser.get_gameObject().SetActive(true);
			}
		}

		private void HideLaser(int machineId)
		{
			_activeWeaponPerMachine.Remove(machineId);
			if (_infoPerMachine.TryGetValue(machineId, out MachinePointerData value))
			{
				Transform laser = value.laser;
				if (laser != null)
				{
					laser.get_gameObject().SetActive(false);
				}
			}
		}

		private void UpdateLaserTarget(int machineId, WeaponWithLaserPointerNode weapon)
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			MachinePointerData machinePointerData = _infoPerMachine[machineId];
			Transform laser = machinePointerData.laser;
			MachineRaycastView machineRaycastView = default(MachineRaycastView);
			if (laser == null || !entityViewsDB.TryQueryEntityView<MachineRaycastView>(weapon.ownerComponent.machineId, ref machineRaycastView))
			{
				return;
			}
			if (weapon.transformComponent.T != null && weapon.transformComponent.T.get_gameObject().get_activeInHierarchy() && TryGetAimPoint(weapon, machineRaycastView.raycastComponent.weaponRaycast.aimPoint, out Vector3 aimPoint, out Vector3 normal, out Vector3 muzzlePosition))
			{
				laser.get_gameObject().SetActive(true);
				laser.set_position(aimPoint);
				if (weapon.ownerComponent.ownedByMe)
				{
					laser.LookAt(aimPoint + normal, _mainCamera.get_position() - aimPoint);
					return;
				}
				laser.LookAt(weapon.transformComponent.T, _mainCamera.get_position() - aimPoint);
				SetRenderSize(machinePointerData.material, Vector3.Distance(aimPoint, muzzlePosition), weapon.projectileRangeStats.maxRange);
			}
			else
			{
				laser.get_gameObject().SetActive(false);
			}
		}

		private bool TryGetAimPoint(WeaponWithLaserPointerNode weapon, Vector3 weaponRaycastAimPoint, out Vector3 aimPoint, out Vector3 normal, out Vector3 muzzlePosition)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			WeaponRaycastUtility.Ray ray = default(WeaponRaycastUtility.Ray);
			Transform val = weapon.muzzleComponent.muzzleFlashLocations[weapon.muzzleComponent.activeMuzzleFlash];
			Vector3 val2 = ray.startPosition = (muzzlePosition = val.get_position());
			val2 = weaponRaycastAimPoint - muzzlePosition;
			ray.direction = val2.get_normalized();
			ray.range = weapon.projectileRangeStats.maxRange;
			WeaponRaycastUtility.Parameters parameters = default(WeaponRaycastUtility.Parameters);
			parameters.machineRootContainer = machineRoots;
			parameters.playerTeamsContainer = playerTeams;
			parameters.playerMachinesContainer = playerMachines;
			parameters.machineManager = machineManager;
			parameters.fusionShieldTag = ((!weapon.ownerComponent.isEnemy) ? WeaponRaycastUtility.ENEMY_FUSION_SHIELD_TAG : WeaponRaycastUtility.ALLY_FUSION_SHIELD_TAG);
			parameters.shooterId = weapon.ownerComponent.ownerId;
			parameters.isShooterAi = false;
			WeaponRaycastUtility.Parameters parameters2 = parameters;
			HitResult hitResult = default(HitResult);
			hitResult.normal = Vector3.get_up();
			HitResult hitResult2 = hitResult;
			bool result = WeaponRaycastUtility.RaycastWeaponAim(ref ray, ref parameters2, ref hitResult2);
			aimPoint = hitResult2.hitPoint;
			normal = hitResult2.normal;
			return result;
		}

		private void SetRenderSize(Material material, float distance, float maxDistance)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			float num = 1f;
			if (distance < maxDistance)
			{
				num = ((!(distance < _data.lasersLength * 2f)) ? 0f : (distance / (_data.lasersLength * 2f) - 1f));
			}
			material.set_mainTextureOffset(new Vector2(0f, num));
		}
	}
}
