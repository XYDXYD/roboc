using Battle;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.AeroFlak
{
	internal sealed class AeroflakProjectileEngine : SingleEntityViewEngine<AeroflakProjectileNode>, IQueryingEntityViewEngine, IInitialize, IPhysicallyTickable, IEngine, ITickableBase
	{
		internal struct CubesHitData
		{
			public InstantiatedCube cubeHit;

			public int damage;

			public int machineId;
		}

		private HashSet<Rigidbody> _checkedMachines = new HashSet<Rigidbody>();

		private BroadcastMissClientCommand _broadcastMissClientCommand;

		private WeaponFireClientCommand _weaponFireClientCommand;

		private WeaponFireEffectOnlyClientCommand _weaponFireEffectOnlyClientCommand;

		private WeaponFireNoEffectClientCommand _weaponFireNoEffectClientCommand;

		private FireMissDependency _fireMissDependency = new FireMissDependency();

		private DestroyCubeDependency _weaponFireDependency = new DestroyCubeDependency();

		private DestroyCubeNoEffectDependency _weaponFireNoEffectDependency = new DestroyCubeNoEffectDependency();

		private DestroyCubeEffectOnlyDependency _weaponFireEffectOnlyDependency = new DestroyCubeEffectOnlyDependency();

		private FasterList<int> _keysToRemove = new FasterList<int>();

		private Dictionary<int, WeaponRaycastUtility.Parameters> _raycastParameters = new Dictionary<int, WeaponRaycastUtility.Parameters>();

		private Dictionary<InstantiatedCube, int> _proposedDestroyedCubes = new Dictionary<InstantiatedCube, int>(30);

		private List<HitCubeInfo> _destroyedCubes = new List<HitCubeInfo>();

		[Inject]
		public NetworkMachineManager networkMachineManager
		{
			get;
			private set;
		}

		[Inject]
		public RigidbodyDataContainer rigidbodyDataContainer
		{
			get;
			private set;
		}

		[Inject]
		public MachineRootContainer machineRootContainer
		{
			get;
			private set;
		}

		[Inject]
		public PlayerMachinesContainer playerMachinesContainer
		{
			get;
			private set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			get;
			private set;
		}

		[Inject]
		public CubeDamagePropagator cubeDamagePropagator
		{
			get;
			private set;
		}

		[Inject]
		public ICommandFactory commandFactory
		{
			get;
			private set;
		}

		[Inject]
		public GameObjectPool gameObjectPool
		{
			get;
			private set;
		}

		[Inject]
		public MachinePreloader machinePreloader
		{
			get;
			private set;
		}

		[Inject]
		public BattleTimer battleTimer
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_weaponFireClientCommand = commandFactory.Build<WeaponFireClientCommand>();
			_weaponFireNoEffectClientCommand = commandFactory.Build<WeaponFireNoEffectClientCommand>();
			_weaponFireEffectOnlyClientCommand = commandFactory.Build<WeaponFireEffectOnlyClientCommand>();
			_broadcastMissClientCommand = commandFactory.Build<BroadcastMissClientCommand>();
		}

		protected override void Add(AeroflakProjectileNode obj)
		{
			_keysToRemove.Add(obj.get_ID());
			IProjectileOwnerComponent projectileOwnerComponent = obj.projectileOwnerComponent;
			WeaponRaycastUtility.Parameters parameters = default(WeaponRaycastUtility.Parameters);
			parameters.machineRootContainer = machineRootContainer;
			parameters.playerTeamsContainer = playerTeamsContainer;
			parameters.playerMachinesContainer = playerMachinesContainer;
			parameters.machineManager = networkMachineManager;
			parameters.fusionShieldTag = ((!projectileOwnerComponent.isEnemy) ? WeaponRaycastUtility.ENEMY_FUSION_SHIELD_TAG : WeaponRaycastUtility.ALLY_FUSION_SHIELD_TAG);
			parameters.shooterId = projectileOwnerComponent.ownerId;
			parameters.isShooterAi = projectileOwnerComponent.isAi;
			WeaponRaycastUtility.Parameters value = parameters;
			_raycastParameters.Add(obj.get_ID(), value);
		}

		protected override void Remove(AeroflakProjectileNode obj)
		{
			_raycastParameters.Remove(obj.get_ID());
		}

		public void Ready()
		{
		}

		public void PhysicsTick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<AeroflakProjectileNode> val = entityViewsDB.QueryEntityViews<AeroflakProjectileNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				AeroflakProjectileNode aeroflakProjectileNode = val.get_Item(i);
				if (_raycastParameters.ContainsKey(aeroflakProjectileNode.get_ID()))
				{
					WeaponRaycastUtility.Parameters raycastParams = _raycastParameters[aeroflakProjectileNode.get_ID()];
					UpdateProjectile(aeroflakProjectileNode, ref raycastParams);
				}
			}
		}

		private static bool MachineBehindFirePoint(Vector3 machinePosition, IProjectileMovementStatsComponent movementStatsComponent)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = machinePosition - movementStatsComponent.startPosition;
			return Vector3.Dot(val, movementStatsComponent.startVelocity) < 0f;
		}

		private void UpdateProjectile(AeroflakProjectileNode aeroflakProjectile, ref WeaponRaycastUtility.Parameters raycastParams)
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			Transform t = aeroflakProjectile.transformComponent.T;
			if (t.get_gameObject().get_activeInHierarchy() && aeroflakProjectile.projectileAliveComponent.active)
			{
				IProjectileMovementStatsComponent projectileMovementStats = aeroflakProjectile.projectileMovementStats;
				IProjectileTimeComponent projectileTimeComponent = aeroflakProjectile.projectileTimeComponent;
				IProjectileOwnerComponent projectileOwnerComponent = aeroflakProjectile.projectileOwnerComponent;
				IProjectileAliveComponent projectileAliveComponent = aeroflakProjectile.projectileAliveComponent;
				if (projectileAliveComponent.justFired)
				{
					float num = Time.get_timeSinceLevelLoad() - projectileTimeComponent.startTime;
					projectileTimeComponent.startTime = Time.get_timeSinceLevelLoad();
					projectileAliveComponent.justFired = false;
					Vector3 position = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, projectileOwnerComponent.machineId).get_position();
					position += rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, projectileOwnerComponent.machineId).get_velocity() * num;
					Vector3 robotStartPosition = projectileMovementStats.robotStartPosition;
					Vector3 val = position - robotStartPosition;
					IProjectileMovementStatsComponent projectileMovementStatsComponent = projectileMovementStats;
					projectileMovementStatsComponent.startPosition += val;
				}
				float timeElapsed = Time.get_timeSinceLevelLoad() - projectileTimeComponent.startTime;
				if (!CheckForCollision(timeElapsed, aeroflakProjectile, projectileMovementStats, ref raycastParams))
				{
					t.set_position(CalculatePosition(timeElapsed, projectileMovementStats.startPosition, projectileMovementStats.startVelocity));
				}
			}
		}

		private Vector3 CalculatePosition(float timeElapsed, Vector3 startPosition, Vector3 startVelocity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = startVelocity * timeElapsed;
			return startPosition + val;
		}

		private bool CheckForCollision(float timeElapsed, AeroflakProjectileNode projectile, IProjectileMovementStatsComponent movementStatsComponent, ref WeaponRaycastUtility.Parameters raycastParams)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			IProjectileMovementStatsComponent projectileMovementStats = projectile.projectileMovementStats;
			IProjectileTimeComponent projectileTimeComponent = projectile.projectileTimeComponent;
			Vector3 val = CalculatePosition(timeElapsed, projectileMovementStats.startPosition, projectileMovementStats.startVelocity);
			Vector3 nextPos = CalculatePosition(timeElapsed + Time.get_fixedDeltaTime(), projectileMovementStats.startPosition, projectileMovementStats.startVelocity);
			return CheckForImpact(val, val, nextPos, projectile, ref raycastParams);
		}

		private bool CheckForImpact(Vector3 currentPos, Vector3 enemyPos, Vector3 nextPos, AeroflakProjectileNode projectile, ref WeaponRaycastUtility.Parameters raycastParams)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			IProjectileMovementStatsComponent projectileMovementStats = projectile.projectileMovementStats;
			Vector3 val = currentPos - projectileMovementStats.startPosition;
			float sqrMagnitude = val.get_sqrMagnitude();
			if (sqrMagnitude > projectile.projectileRangeComponent.maxRange * projectile.projectileRangeComponent.maxRange)
			{
				DisableProjectile(projectile.get_ID());
				return false;
			}
			if (CheckForProximityHit(enemyPos, nextPos, projectile))
			{
				return true;
			}
			if (CheckForDirectHit(currentPos, enemyPos, nextPos, projectile))
			{
				return true;
			}
			return false;
		}

		private bool CheckForDirectHit(Vector3 currentPos, Vector3 enemyPos, Vector3 nextPos, AeroflakProjectileNode projectile)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			int iD = projectile.get_ID();
			Vector3 direction;
			float distance;
			WeaponRaycastUtility.Ray rayMe = CreateRay(currentPos, nextPos, out direction, out distance);
			HitResult hitResult = default(HitResult);
			hitResult.hitPoint = currentPos;
			hitResult.normal = -direction;
			HitResult hitResult2 = hitResult;
			WeaponRaycastUtility.Parameters parameters = _raycastParameters[iD];
			IProjectileOwnerComponent projectileOwnerComponent = projectile.projectileOwnerComponent;
			parameters.Inject(projectileOwnerComponent.ownerId, projectileOwnerComponent.isAi, (!projectileOwnerComponent.isEnemy) ? WeaponRaycastUtility.ENEMY_FUSION_SHIELD_TAG : WeaponRaycastUtility.ALLY_FUSION_SHIELD_TAG);
			if (WeaponRaycastUtility.RaycastWeaponHit(ref rayMe, ref rayMe, ref parameters, ref hitResult2))
			{
				projectile.transformComponent.T.set_position(hitResult2.hitPoint);
				TargetType targetType = hitResult2.targetType;
				bool flag = true;
				bool flag2 = false;
				if (LayerToTargetType.IsTargetDestructible(targetType))
				{
					Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitResult2.hitTargetMachineId);
					if (rigidBodyData != null)
					{
						int playerId = GetPlayerId(rigidBodyData, targetType);
						flag2 = IsOnOwnersTeam(playerId, targetType, projectile.projectileOwnerComponent);
						if (!flag2)
						{
							flag = false;
							if (GameUtility.MachineIsOnGround(rigidBodyData, targetType, projectile.aeroflakProjectileStats.groundClearance, machineRootContainer, machinePreloader))
							{
								DirectHit(hitResult2, projectile);
							}
							else
							{
								ProximityHit(ref hitResult2, hitResult2.hitPoint, playerId, targetType, projectile);
							}
						}
					}
				}
				if (flag)
				{
					HitProp(hitResult2.hitPoint, hitResult2.normal, flag2, targetType, projectile);
				}
				DisableProjectile(iD);
				return true;
			}
			return false;
		}

		private bool CheckForProximityHit(Vector3 currentPos, Vector3 nextPos, AeroflakProjectileNode projectile)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			IProjectileMovementStatsComponent projectileMovementStats = projectile.projectileMovementStats;
			IAeroflakProjectileStatsComponent aeroflakProjectileStats = projectile.aeroflakProjectileStats;
			float targetSpeed = projectileMovementStats.speed * Time.get_deltaTime();
			_checkedMachines.Clear();
			Collider[] array = Physics.OverlapSphere(currentPos, aeroflakProjectileStats.explosionRadius, GameLayers.ALL_PLAYERS_LAYER_MASK);
			TargetType targetType = TargetType.Player;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject machineBoard = GameUtility.GetMachineBoard(array[i].get_transform());
				int machineIdFromRoot = machineRootContainer.GetMachineIdFromRoot(targetType, machineBoard);
				Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, machineIdFromRoot);
				if (IsMachineValidForDamage(rigidBodyData, targetType, machineIdFromRoot, projectile) && CheckClosestMachine(rigidBodyData, currentPos, nextPos, targetSpeed, targetType, projectile))
				{
					DisableProjectile(projectile.get_ID());
					return true;
				}
			}
			return false;
		}

		private bool IsMachineValidForDamage(Rigidbody hitMachine, TargetType targetType, int hitMachineId, AeroflakProjectileNode projectile)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (_checkedMachines.Contains(hitMachine))
			{
				return false;
			}
			_checkedMachines.Add(hitMachine);
			if (!CheckHitMachineVisibility(hitMachineId))
			{
				return false;
			}
			if (MachineBehindFirePoint(hitMachine.get_worldCenterOfMass(), projectile.projectileMovementStats))
			{
				return false;
			}
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(targetType, hitMachineId);
			if (IsOnOwnersTeam(playerFromMachineId, targetType, projectile.projectileOwnerComponent))
			{
				return false;
			}
			return true;
		}

		private bool CheckHitMachineVisibility(int hitMachineId)
		{
			MachineInvisibilityNode machineInvisibilityNode = default(MachineInvisibilityNode);
			if (entityViewsDB.TryQueryEntityView<MachineInvisibilityNode>(hitMachineId, ref machineInvisibilityNode) && !machineInvisibilityNode.machineVisibilityComponent.isVisible)
			{
				return false;
			}
			return true;
		}

		private bool CheckClosestMachine(Rigidbody hitMachine, Vector3 currentPos, Vector3 nextPos, float targetSpeed, TargetType targetType, AeroflakProjectileNode projectile)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			if (GameUtility.MachineIsOnGround(hitMachine, targetType, projectile.aeroflakProjectileStats.groundClearance, machineRootContainer, machinePreloader))
			{
				return false;
			}
			Vector3 val = hitMachine.get_worldCenterOfMass() - currentPos;
			float sqrMagnitude = val.get_sqrMagnitude();
			Vector3 val2 = hitMachine.get_worldCenterOfMass() + hitMachine.get_velocity() * Time.get_deltaTime() - nextPos;
			float sqrMagnitude2 = val2.get_sqrMagnitude();
			if (sqrMagnitude2 > sqrMagnitude)
			{
				int playerId = GetPlayerId(hitMachine, targetType);
				Vector3 position = MathUtility.ClosetPointOnLine(currentPos, nextPos, hitMachine.get_worldCenterOfMass());
				HitResult hitResult = default(HitResult);
				hitResult.targetType = TargetType.Environment;
				hitResult.normal = -projectile.transformComponent.T.get_forward();
				ProximityHit(ref hitResult, position, playerId, targetType, projectile);
				return true;
			}
			return false;
		}

		private void DisableProjectile(int id)
		{
			_keysToRemove.Add(id);
			AeroflakProjectileNode aeroflakProjectileNode = default(AeroflakProjectileNode);
			if (entityViewsDB.TryQueryEntityView<AeroflakProjectileNode>(_keysToRemove.get_Item(_keysToRemove.get_Count() - 1), ref aeroflakProjectileNode))
			{
				int value = aeroflakProjectileNode.get_ID();
				aeroflakProjectileNode.projectileAliveComponent.resetProjectile.Dispatch(ref value);
				aeroflakProjectileNode.projectileAliveComponent.active = false;
			}
		}

		private bool IsOnOwnersTeam(int playerId, TargetType targetType, IProjectileOwnerComponent projectileOwnerComponent)
		{
			if (playerId < 0)
			{
				return false;
			}
			int playerTeam = playerTeamsContainer.GetPlayerTeam(targetType, playerId);
			return projectileOwnerComponent.ownerTeam == playerTeam;
		}

		private int GetPlayerId(Rigidbody hitMachine, TargetType targetType)
		{
			if (hitMachine == null)
			{
				return -1;
			}
			GameObject machineBoard = GameUtility.GetMachineBoard(hitMachine.get_transform());
			int machineIdFromRoot = machineRootContainer.GetMachineIdFromRoot(targetType, machineBoard);
			return playerMachinesContainer.GetPlayerFromMachineId(targetType, machineIdFromRoot);
		}

		private List<HitCubeInfo> GenerateHitCubesDirectHit(InstantiatedCube hitCubeInstance, HitResult hitResult, IProjectileDamageStatsComponent projectileStats)
		{
			_destroyedCubes.Clear();
			Dictionary<InstantiatedCube, int> proposedDamage = cubeDamagePropagator.GetProposedDamage(hitCubeInstance, projectileStats.damageBoost, projectileStats.damage, projectileStats.damageBuff, projectileStats.damageMultiplier, projectileStats.protoniumDamageScale, projectileStats.campaignDifficultyFactor);
			cubeDamagePropagator.GenerateDestructionGroupHitInfo(proposedDamage, _destroyedCubes);
			return _destroyedCubes;
		}

		private void ProximityHit(ref HitResult hitResult, Vector3 position, int hitPlayerId, TargetType targetType, AeroflakProjectileNode projectile)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			if (projectile.entitySourceComponent.isLocal)
			{
				projectile.stackDamageComponent.stackableHit.set_value(true);
				WeaponSplashDamageUtility.HitCubesResultList cubesInSphere = GetCubesInSphere(ref hitResult, position, projectile.aeroflakProjectileStats.damageRadius, projectile);
				bool flag = hitPlayerId == projectile.projectileOwnerComponent.ownerId;
				hitResult.hitPoint = position;
				if (!ApplyDamage(projectile, ref hitResult, cubesInSphere.playerMachines, TargetType.Player))
				{
					targetType = TargetType.Environment;
					CreateMissCommand(position, -projectile.transformComponent.T.get_forward(), explosion: true, flag, TargetType.Environment, projectile);
				}
				else
				{
					targetType = TargetType.Player;
				}
				int currentStackIndex = projectile.stackDamageComponent.currentStackIndex;
				int stackCount_ = currentStackIndex * 100 / projectile.stackDamageComponent.buffMaxStacks;
				bool targetIsMe_ = playerTeamsContainer.IsMe(targetType, hitPlayerId);
				HitInfo value = new HitInfo(targetType, projectile.itemDescriptorComponent.itemDescriptor, projectile.projectileOwnerComponent.isEnemy, hit_: true, flag, position, Quaternion.get_identity(), Vector3.get_up(), targetIsMe_, shooterIsMe_: true, targetOnMyTeam_: false, null, stackCount_);
				projectile.hitSomethingComponent.hitEnemySplash.Dispatch(ref value);
			}
		}

		private void DirectHit(HitResult hitResult, AeroflakProjectileNode projectile)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			if (projectile.entitySourceComponent.isLocal)
			{
				Vector3 hitPoint = hitResult.hitPoint;
				TargetType targetType = hitResult.targetType;
				IMachineMap machineMap = networkMachineManager.GetMachineMap(targetType, hitResult.hitTargetMachineId);
				int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(targetType, hitResult.hitTargetMachineId);
				Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitResult.hitTargetMachineId);
				InstantiatedCube cubeAtGridPosition = MachineUtility.GetCubeAtGridPosition(hitResult.gridHit.hitGridPos, machineMap);
				List<HitCubeInfo> hitCubes = GenerateHitCubesDirectHit(cubeAtGridPosition, hitResult, projectile.projectileDamageStats);
				int weaponDamage = CubeDamagePropagator.GetWeaponDamage(projectile.projectileDamageStats);
				CreateFireCommandDirectHit(hitResult.hitTargetMachineId, hitCubes, hitResult.hitPoint, targetType, projectile.transformComponent.T.get_position(), hitResult.normal, projectile.projectileOwnerComponent.machineId, projectile.itemDescriptorComponent.itemDescriptor, weaponDamage);
				bool targetIsMe_ = playerTeamsContainer.IsMe(targetType, playerFromMachineId);
				bool shooterIsMe_ = playerTeamsContainer.IsMe(targetType, projectile.projectileOwnerComponent.ownerId);
				HitInfo value = new HitInfo(targetType, projectile.itemDescriptorComponent.itemDescriptor, projectile.projectileOwnerComponent.isEnemy, hit_: true, hitSelf_: false, hitPoint, Quaternion.get_identity(), hitResult.normal, targetIsMe_, shooterIsMe_, targetOnMyTeam_: false, rigidBodyData);
				switch (targetType)
				{
				case TargetType.TeamBase:
					projectile.hitSomethingComponent.hitProtonium.Dispatch(ref value);
					break;
				case TargetType.EqualizerCrystal:
					projectile.hitSomethingComponent.hitEqualizer.Dispatch(ref value);
					break;
				default:
					projectile.hitSomethingComponent.hitEnemy.Dispatch(ref value);
					break;
				}
			}
		}

		private void HitProp(Vector3 position, Vector3 normal, bool hitTeamMate, TargetType targetType, AeroflakProjectileNode projectile)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			if (projectile.entitySourceComponent.isLocal)
			{
				CreateMissCommand(position, normal, explosion: false, hitTeamMate, targetType, projectile);
				Quaternion rotation_ = Quaternion.FromToRotation(Vector3.get_zero(), normal);
				HitInfo value = new HitInfo(targetType, projectile.itemDescriptorComponent.itemDescriptor, projectile.projectileOwnerComponent.isEnemy, hit_: false, hitTeamMate, position, rotation_, Vector3.get_up());
				if (hitTeamMate)
				{
					projectile.hitSomethingComponent.hitSelf.Dispatch(ref value);
				}
				else if (targetType == TargetType.FusionShield)
				{
					projectile.hitSomethingComponent.hitFusionShield.Dispatch(ref value);
				}
				else
				{
					projectile.hitSomethingComponent.hitEnvironment.Dispatch(ref value);
				}
			}
		}

		private WeaponSplashDamageUtility.HitCubesResultList GetCubesInSphere(ref HitResult hitResult, Vector3 pos, float damageRadius, AeroflakProjectileNode projectile)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			WeaponSplashDamageUtility.SplashParameters splashParameters = default(WeaponSplashDamageUtility.SplashParameters);
			splashParameters.position = pos;
			splashParameters.radius = damageRadius;
			splashParameters.direction = projectile.transformComponent.T.get_forward();
			splashParameters.additionalHits = projectile.splashComponent.additionalHits;
			splashParameters.coneAngle = projectile.splashComponent.coneAngle;
			WeaponSplashDamageUtility.Parameters parameters = default(WeaponSplashDamageUtility.Parameters);
			parameters.machineRootContainer = machineRootContainer;
			parameters.rigidbodyDataContainer = rigidbodyDataContainer;
			parameters.machineManager = networkMachineManager;
			parameters.playerMachinesContainer = playerMachinesContainer;
			parameters.playerTeamsContainer = playerTeamsContainer;
			parameters.projectileOwnerTeam = projectile.projectileOwnerComponent.ownerTeam;
			WeaponSplashDamageUtility.Parameters parameters2 = parameters;
			return WeaponSplashDamageUtility.SplashDamageCubesList(ref splashParameters, ref parameters2, ref hitResult);
		}

		private bool ApplyDamage(AeroflakProjectileNode projectile, ref HitResult hitResult, Dictionary<int, FasterList<InstantiatedCube>> targets, TargetType type)
		{
			bool result = false;
			bool flag = false;
			_destroyedCubes.Clear();
			if (targets.Count > 0)
			{
				foreach (KeyValuePair<int, FasterList<InstantiatedCube>> target in targets)
				{
					int key = target.Key;
					FasterList<InstantiatedCube> value = target.Value;
					ProcessHitCubes(value, projectile);
					CreateFireCommandNoEffect(type, key, projectile);
					if (!flag)
					{
						flag = true;
						CreateCubeDestroyEffectOnlyCommand(type, key, ref hitResult, projectile);
					}
					_destroyedCubes.Clear();
					result = true;
				}
				return result;
			}
			return result;
		}

		private void ProcessHitCubes(FasterList<InstantiatedCube> hitCubes, AeroflakProjectileNode projectile)
		{
			IAeroflakProjectileStatsComponent aeroflakProjectileStats = projectile.aeroflakProjectileStats;
			IProjectileDamageStatsComponent projectileDamageStats = projectile.projectileDamageStats;
			IStackDamageComponent stackDamageComponent = projectile.stackDamageComponent;
			_proposedDestroyedCubes.Clear();
			float num = projectileDamageStats.damageMultiplier * projectileDamageStats.damageBoost * projectileDamageStats.damageBuff * projectileDamageStats.campaignDifficultyFactor * (float)(aeroflakProjectileStats.damageProximityHit + stackDamageComponent.currentStackIndex * stackDamageComponent.buffDamagePerStack);
			for (int i = 0; i < hitCubes.get_Count(); i++)
			{
				InstantiatedCube target = hitCubes.get_Item(i);
				int damage = Mathf.CeilToInt(num / (float)hitCubes.get_Count());
				cubeDamagePropagator.ComputeProposedDamage(target, damage, projectileDamageStats.protoniumDamageScale, ref _proposedDestroyedCubes);
			}
			if (_proposedDestroyedCubes.Count > 0)
			{
				cubeDamagePropagator.GenerateDestructionGroupHitInfo(_proposedDestroyedCubes, _destroyedCubes);
			}
		}

		private void CreateFireCommandNoEffect(TargetType targetType, int hitTargetMachineId, AeroflakProjectileNode projectile)
		{
			if (_destroyedCubes.Count > 0)
			{
				_weaponFireNoEffectDependency.SetVariables(projectile.projectileOwnerComponent.machineId, hitTargetMachineId, _destroyedCubes, battleTimer.SecondsSinceGameInitialised, targetType);
				_weaponFireNoEffectClientCommand.Inject(_weaponFireNoEffectDependency).Execute();
			}
		}

		private void CreateFireCommandDirectHit(int hitMachineId, List<HitCubeInfo> hitCubes, Vector3 hitPoint, TargetType targetType, Vector3 pos, Vector3 normal, int machineId, ItemDescriptor itemDescriptor, int damage)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			if (hitCubes.Count > 0)
			{
				HitCubeInfo hitCubeInfo = hitCubes[0];
				Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitMachineId);
				Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(hitCubeInfo.gridLoc, rigidBodyData, targetType);
				Vector3 hitEffectOffset = hitPoint - cubeWorldPosition;
				_weaponFireDependency.SetVariables(machineId, hitMachineId, hitEffectOffset, normal, itemDescriptor, hitCubes, battleTimer.SecondsSinceGameInitialised, targetType, damage);
				_weaponFireClientCommand.Inject(_weaponFireDependency).Execute();
			}
		}

		private void CreateMissCommand(Vector3 position, Vector3 normal, bool explosion, bool hitSelf, TargetType targetType, AeroflakProjectileNode projectileStats)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_fireMissDependency.SetVariables(projectileStats.projectileOwnerComponent.machineId, projectileStats.itemDescriptorComponent.itemDescriptor, position, normal, explosion, hitSelf, battleTimer.SecondsSinceGameInitialised, targetType);
			_broadcastMissClientCommand.Inject(_fireMissDependency);
			_broadcastMissClientCommand.Execute();
		}

		private void CreateCubeDestroyEffectOnlyCommand(TargetType targetType, int hitTargetMachineId, ref HitResult hitResult, AeroflakProjectileNode projectile)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			if (_destroyedCubes.Count > 0)
			{
				Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitTargetMachineId);
				HitCubeInfo hitCubeInfo = _destroyedCubes[0];
				Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(hitCubeInfo.gridLoc, rigidBodyData, targetType);
				Vector3 hitEffectOffset = hitResult.hitPoint - cubeWorldPosition;
				int currentStackIndex = projectile.stackDamageComponent.currentStackIndex;
				int stackCount = currentStackIndex * 100 / projectile.stackDamageComponent.buffMaxStacks;
				DestroyCubeEffectOnlyDependency weaponFireEffectOnlyDependency = _weaponFireEffectOnlyDependency;
				HitCubeInfo hitCubeInfo2 = _destroyedCubes[0];
				weaponFireEffectOnlyDependency.SetVariables(hitCubeInfo2.gridLoc, projectile.projectileOwnerComponent.machineId, hitTargetMachineId, targetType, projectile.itemDescriptorComponent.itemDescriptor, hitEffectOffset, hitResult.normal, stackCount);
				_weaponFireEffectOnlyClientCommand.Inject(_weaponFireEffectOnlyDependency);
				_weaponFireEffectOnlyClientCommand.Execute();
			}
		}

		private WeaponRaycastUtility.Ray CreateRay(Vector3 currentPosition, Vector3 nextPosition, out Vector3 direction, out float distance)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			direction = nextPosition - currentPosition;
			distance = direction.get_magnitude();
			direction /= distance;
			WeaponRaycastUtility.Ray result = default(WeaponRaycastUtility.Ray);
			result.startPosition = currentPosition;
			result.direction = direction;
			result.range = distance;
			return result;
		}

		private void RecycleDeadProjectiles()
		{
			if (_keysToRemove.get_Count() > 0)
			{
				for (int i = 0; i < _keysToRemove.get_Count(); i++)
				{
					AeroflakProjectileNode aeroflakProjectileNode = default(AeroflakProjectileNode);
					if (entityViewsDB.TryQueryEntityView<AeroflakProjectileNode>(_keysToRemove.get_Item(i), ref aeroflakProjectileNode))
					{
						aeroflakProjectileNode.transformComponent.T.get_gameObject().SetActive(false);
					}
				}
			}
			_keysToRemove.FastClear();
		}
	}
}
