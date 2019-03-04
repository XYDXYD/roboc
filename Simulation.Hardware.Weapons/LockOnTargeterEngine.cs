using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class LockOnTargeterEngine : SingleEntityViewEngine<LockOnTargetNode>, IInitialize, ITickable, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private struct LooseLockData
		{
			public Vector3 opponentDirection;

			public float dot;

			public LooseLockData(Vector3 opponentDirection, float dot)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				this.opponentDirection = opponentDirection;
				this.dot = dot;
			}
		}

		private const int LockStages = 3;

		private int _ownerPlayerId;

		private ItemDescriptor _ownerCurrentItemDescriptor;

		private int _ownerMachineId;

		private bool _targeterIsActive;

		private int _lockStage;

		private bool _targetAllies;

		private int _targetPlayerId;

		private int _targetMachineId = -1;

		private InstantiatedCube _lockedCube;

		private Rigidbody _lockedRigidbody;

		private float _maxRange;

		private float _lockOnTime;

		private float _changeLockTime;

		private float _fullLockReleaseTime;

		private bool _looseLock;

		private bool _notifyTargetOfLock;

		private bool _initNeeded;

		private float _lockConeDot;

		private bool _aquiredLock;

		private float _applyNextLockTime;

		private float _releaseTime;

		private int _switchLockMachine;

		private bool _isSwitchingLock;

		private float _startSwitchLockTime;

		private Vector3 centreOfScreen = new Vector3((float)Screen.get_width() * 0.5f, (float)Screen.get_height() * 0.5f, 0f);

		private GridAllignedLineCheck.GridAlignedCheckDependency _dependency = new GridAllignedLineCheck.GridAlignedCheckDependency();

		private Dictionary<ItemDescriptor, LockOnTargetNode> _weaponSubCategoryToNodes = new Dictionary<ItemDescriptor, LockOnTargetNode>(24);

		private FasterList<LooseLockData> _looseLockPotentialtargets = new FasterList<LooseLockData>(12);

		private bool _gameStarted;

		private bool _targetNewCube;

		private LockOnNotifierClientCommand _lockOnNotifier;

		private ItemDescriptor _currentSubCategory;

		private LockOnStateObservable _lockOnStateObservable;

		private HardwareDestroyedObserver _destroyedObserver;

		private HardwareEnabledObserver _enabledObserver;

		private RemotePlayerBecomeInvisibleObserver _playerBecomeInvisibleObserver;

		private WeaponFiredObserver _weaponFireObserver;

		[Inject]
		internal MachineRootContainer machineRootContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
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
		internal CrosshairController crosshairContainer
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal SwitchWeaponObserver switchWeaponObserver
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
		internal LivePlayersContainer livePlayersContainer
		{
			private get;
			set;
		}

		[Inject]
		internal RigidbodyDataContainer rigidbodyContainer
		{
			private get;
			set;
		}

		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		internal GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe LockOnTargeterEngine(LockOnStateObservable lockOnStateObservable, HardwareDestroyedObserver destroyedObserver, HardwareEnabledObserver enabledObserver, RemotePlayerBecomeInvisibleObserver playerBecomeInvisibleObserver, WeaponFiredObserver weaponFireObserver)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			_lockOnStateObservable = lockOnStateObservable;
			_destroyedObserver = destroyedObserver;
			_destroyedObserver.AddAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_enabledObserver = enabledObserver;
			_enabledObserver.AddAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_playerBecomeInvisibleObserver = playerBecomeInvisibleObserver;
			_playerBecomeInvisibleObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_weaponFireObserver = weaponFireObserver;
			_weaponFireObserver.AddAction(new ObserverAction<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		void IInitialize.OnDependenciesInjected()
		{
			_lockOnNotifier = commandFactory.Build<LockOnNotifierClientCommand>();
			switchWeaponObserver.SwitchWeaponsEvent += OnWeaponSwitched;
			destructionReporter.OnPlayerDamageApplied += HandleOnLocalPlayerCubesDestroyed;
			gameStartDispatcher.Register(HandleOnGameStarted);
		}

		protected override void Add(LockOnTargetNode node)
		{
			if (node.owner.ownedByMe)
			{
				_ownerPlayerId = node.owner.ownerId;
				_ownerMachineId = node.owner.machineId;
				_weaponSubCategoryToNodes[node.itemDescriptorComponent.itemDescriptor] = node;
			}
		}

		protected override void Remove(LockOnTargetNode node)
		{
			if (node.owner.ownedByMe)
			{
				ItemDescriptor itemDescriptor = node.itemDescriptorComponent.itemDescriptor;
				if (_weaponSubCategoryToNodes.Remove(itemDescriptor))
				{
					HandleOnSubCategoryDestroyed(ref itemDescriptor);
				}
			}
		}

		public void Ready()
		{
		}

		public void Tick(float deltaSec)
		{
			if (_targeterIsActive && _gameStarted)
			{
				UpdateWeaponLocking();
			}
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			switchWeaponObserver.SwitchWeaponsEvent -= OnWeaponSwitched;
			_destroyedObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_enabledObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			destructionReporter.OnPlayerDamageApplied -= HandleOnLocalPlayerCubesDestroyed;
			_playerBecomeInvisibleObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			gameStartDispatcher.Unregister(HandleOnGameStarted);
			_weaponFireObserver.RemoveAction(new ObserverAction<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void ClearLockedCube(ref float power)
		{
			if (_lockStage == 3)
			{
				_targetNewCube = true;
			}
		}

		private void HandleOnGameStarted()
		{
			_gameStarted = true;
		}

		private void HandleOnPlayerBecomeInvisible(ref int playerId)
		{
			if (_targetPlayerId == playerId && _aquiredLock)
			{
				LoseLock();
			}
		}

		private void HandleOnLocalPlayerCubesDestroyed(DestructionData data)
		{
			if (data.targetIsMe && data.isDestroyed && _aquiredLock)
			{
				LoseLock();
			}
		}

		private void HandleOnSubCategoryDestroyed(ref ItemDescriptor itemDescriptor)
		{
			if (_currentSubCategory != null && ((itemDescriptor.itemCategory == ItemCategory.Seeker && _currentSubCategory.itemCategory == ItemCategory.Seeker) || (itemDescriptor.itemCategory == ItemCategory.Nano && _currentSubCategory.itemCategory == ItemCategory.Nano)))
			{
				_targeterIsActive = false;
				if (_lockStage > 0)
				{
					LoseLock();
				}
			}
		}

		private void HandleOnSubCategoryEnabled(ref ItemDescriptor itemDescriptor)
		{
			if (_currentSubCategory != null && ((_currentSubCategory.itemCategory == ItemCategory.Seeker && itemDescriptor.itemCategory == ItemCategory.Seeker) || (_currentSubCategory.itemCategory == ItemCategory.Nano && itemDescriptor.itemCategory == ItemCategory.Nano)))
			{
				_targeterIsActive = true;
				_initNeeded = true;
				_targetAllies = (itemDescriptor.itemCategory == ItemCategory.Nano);
			}
		}

		private void UpdateWeaponLocking()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			if (_initNeeded)
			{
				InitData(_currentSubCategory);
			}
			Ray ray = Camera.get_main().ScreenPointToRay(centreOfScreen);
			int sHOOTING_OTHERS_LAYER_MASK = GameLayers.SHOOTING_OTHERS_LAYER_MASK;
			RaycastHit rcHit = default(RaycastHit);
			bool flag = Physics.Raycast(ray.get_origin(), ray.get_direction(), ref rcHit, _maxRange, sHOOTING_OTHERS_LAYER_MASK);
			bool flag2 = false;
			if (flag && LayerToTargetType.GetType(rcHit.get_collider().get_gameObject().get_layer()) == TargetType.Player)
			{
				Transform transform = rcHit.get_collider().get_gameObject().get_transform();
				int enemyMachineId = MachineIdFromTransform(transform);
				flag2 = CheckMachineVisibilityForExclusion(enemyMachineId);
			}
			if (!_aquiredLock)
			{
				if (flag && LayerToTargetType.GetType(rcHit.get_collider().get_gameObject().get_layer()) == TargetType.Player && !flag2)
				{
					Transform transform2 = rcHit.get_collider().get_gameObject().get_transform();
					int num = MachineIdFromTransform(transform2);
					int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, num);
					if (playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerFromMachineId) == _targetAllies)
					{
						BeginLock(playerFromMachineId, num, ref rcHit, ref ray);
					}
				}
				else if (_looseLock)
				{
					ApplyLooseLock();
				}
			}
			else if (!livePlayersContainer.IsPlayerAlive(TargetType.Player, _targetPlayerId))
			{
				LoseLock();
			}
			else if ((_lockedCube == null || _lockedCube.isDestroyed || _targetNewCube) && _targetMachineId >= 0)
			{
				IMachineMap machineMap = machineManager.GetMachineMap(TargetType.Player, _targetMachineId);
				HashSet<InstantiatedCube> remainingCubes = machineMap.GetRemainingCubes();
				if (remainingCubes.Count > 0)
				{
					HashSet<InstantiatedCube>.Enumerator enumerator = remainingCubes.GetEnumerator();
					int num2 = Random.Range(0, remainingCubes.Count);
					enumerator.MoveNext();
					for (int i = 0; i < num2; i++)
					{
						enumerator.MoveNext();
					}
					_lockedCube = enumerator.Current;
					crosshairContainer.lockTargetPosition = _lockedRigidbody.get_worldCenterOfMass();
					if (_lockStage == 3 && _aquiredLock)
					{
						SendLockOnNotification(force: true);
					}
				}
				else
				{
					LoseLock();
				}
				_targetNewCube = false;
			}
			else
			{
				if (!HasSustainedLock(flag, ref rcHit, ref ray, flag2) || !flag)
				{
					return;
				}
				if (LayerToTargetType.GetType(rcHit.get_collider().get_gameObject().get_layer()) == TargetType.Player && !flag2)
				{
					int num3 = MachineIdFromTransform(rcHit.get_collider().get_gameObject().get_transform());
					if (num3 != _targetMachineId)
					{
						if (!_isSwitchingLock || _switchLockMachine != num3)
						{
							_isSwitchingLock = true;
							_startSwitchLockTime = Time.get_timeSinceLevelLoad() + _changeLockTime;
							_switchLockMachine = num3;
						}
						else if (Time.get_timeSinceLevelLoad() >= _startSwitchLockTime)
						{
							LoseLock();
						}
					}
				}
				else
				{
					_isSwitchingLock = false;
				}
			}
		}

		private void ApplyLooseLock()
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			IList<int> list = (IList<int>)((!_targetAllies) ? ((object)playerTeamsContainer.GetPlayersOnEnemyTeam(TargetType.Player)) : ((object)playerTeamsContainer.GetPlayersOnTeam(TargetType.Player, playerTeamsContainer.GetMyTeam())));
			Ray ray = Camera.get_main().ScreenPointToRay(centreOfScreen);
			Vector3 origin = ray.get_origin();
			Vector3 direction = ray.get_direction();
			Vector3 normalized = direction.get_normalized();
			for (int i = 0; i < list.Count; i++)
			{
				int num = list[i];
				int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, num);
				bool flag = false;
				flag = CheckMachineVisibilityForExclusion(activeMachine);
				if (!livePlayersContainer.IsPlayerAlive(TargetType.Player, num) || flag || !machineRootContainer.IsMachineRegistered(TargetType.Player, activeMachine))
				{
					continue;
				}
				Rigidbody rigidBodyData = rigidbodyContainer.GetRigidBodyData(TargetType.Player, activeMachine);
				Vector3 val = rigidBodyData.get_worldCenterOfMass() - origin;
				Vector3 normalized2 = val.get_normalized();
				float num2 = Vector3.Dot(normalized, normalized2);
				if (!(num2 > _lockConeDot))
				{
					continue;
				}
				LooseLockData looseLockData = new LooseLockData(normalized2, num2);
				bool flag2 = true;
				for (int j = 0; j < _looseLockPotentialtargets.get_Count(); j++)
				{
					LooseLockData looseLockData2 = _looseLockPotentialtargets.get_Item(j);
					if (looseLockData2.dot < num2)
					{
						_looseLockPotentialtargets.Insert(j, looseLockData);
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					_looseLockPotentialtargets.Add(looseLockData);
				}
			}
			if (_looseLockPotentialtargets.get_Count() <= 0)
			{
				return;
			}
			for (int k = 0; k < _looseLockPotentialtargets.get_Count(); k++)
			{
				int sHOOTING_OTHERS_LAYER_MASK = GameLayers.SHOOTING_OTHERS_LAYER_MASK;
				Vector3 origin2 = ray.get_origin();
				LooseLockData looseLockData3 = _looseLockPotentialtargets.get_Item(k);
				RaycastHit rcHit = default(RaycastHit);
				if (Physics.Raycast(origin2, looseLockData3.opponentDirection, ref rcHit, _maxRange, sHOOTING_OTHERS_LAYER_MASK) && LayerToTargetType.GetType(rcHit.get_collider().get_gameObject().get_layer()) == TargetType.Player)
				{
					Transform transform = rcHit.get_collider().get_gameObject().get_transform();
					int num3 = MachineIdFromTransform(transform);
					int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, num3);
					if (playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerFromMachineId) == _targetAllies)
					{
						BeginLock(playerFromMachineId, num3, ref rcHit, ref ray);
						break;
					}
				}
			}
			_looseLockPotentialtargets.FastClear();
		}

		private bool CheckMachineVisibilityForExclusion(int enemyMachineId)
		{
			MachineInvisibilityNode machineInvisibilityNode = default(MachineInvisibilityNode);
			if (entityViewsDB.TryQueryEntityView<MachineInvisibilityNode>(enemyMachineId, ref machineInvisibilityNode))
			{
				return !machineInvisibilityNode.machineVisibilityComponent.isVisible;
			}
			return false;
		}

		private bool HasSustainedLock(bool didHit, ref RaycastHit mainRcHit, ref Ray ray, bool playerExcluded)
		{
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			bool flag2 = false;
			float num = 1f;
			if (didHit && LayerToTargetType.GetType(mainRcHit.get_collider().get_gameObject().get_layer()) == TargetType.Player)
			{
				int num2 = MachineIdFromTransform(mainRcHit.get_collider().get_gameObject().get_transform());
				if (num2 == _targetMachineId && TryGetCubeInLine(ref mainRcHit, ref ray, num2, out InstantiatedCube cube))
				{
					_lockedCube = cube;
					_lockedRigidbody = mainRcHit.get_collider().get_attachedRigidbody();
					flag2 = true;
				}
			}
			if (!flag2)
			{
				Ray val = Camera.get_main().ScreenPointToRay(centreOfScreen);
				Vector3 val2 = GetLockWorldPosition() - val.get_origin();
				Vector3 normalized = val2.get_normalized();
				Vector3 direction = val.get_direction();
				num = Vector3.Dot(direction.get_normalized(), normalized);
				if (_lockedRigidbody.get_gameObject().get_layer() != GameLayers.IGNORE_RAYCAST)
				{
					int sHOOTING_OTHERS_LAYER_MASK = GameLayers.SHOOTING_OTHERS_LAYER_MASK;
					RaycastHit val3 = default(RaycastHit);
					if (Physics.Raycast(val.get_origin(), normalized, ref val3, _maxRange, sHOOTING_OTHERS_LAYER_MASK))
					{
						if (LayerToTargetType.GetType(val3.get_collider().get_gameObject().get_layer()) == TargetType.Player)
						{
							int num3 = MachineIdFromTransform(val3.get_collider().get_gameObject().get_transform());
							if (num3 != _targetMachineId)
							{
								flag = true;
							}
						}
						else
						{
							flag = true;
						}
					}
				}
			}
			crosshairContainer.lockTargetPosition = _lockedRigidbody.get_worldCenterOfMass();
			float num4 = Vector3.SqrMagnitude(_lockedRigidbody.get_worldCenterOfMass() - ray.get_origin());
			if ((flag2 || (!flag && num > _lockConeDot && num4 <= _maxRange * _maxRange)) && !playerExcluded)
			{
				if (_lockStage < 3 && Time.get_timeSinceLevelLoad() > _applyNextLockTime)
				{
					_applyNextLockTime = Time.get_timeSinceLevelLoad() + _lockOnTime / 2f;
					_lockStage++;
					crosshairContainer.lockStage = _lockStage;
					SendLockOnNotification();
				}
				if (_lockStage == 3)
				{
					_releaseTime = Time.get_timeSinceLevelLoad() + _fullLockReleaseTime;
				}
				return true;
			}
			if (!flag && Time.get_timeSinceLevelLoad() < _releaseTime)
			{
				return true;
			}
			LoseLock();
			return false;
		}

		private void BeginLock(int lockedPlayerId, int lockedMachineId, ref RaycastHit rcHit, ref Ray ray)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			if (TryGetCubeInLine(ref rcHit, ref ray, lockedMachineId, out InstantiatedCube cube))
			{
				_lockedCube = cube;
				_targetPlayerId = lockedPlayerId;
				_targetMachineId = lockedMachineId;
				_lockedRigidbody = rcHit.get_collider().get_attachedRigidbody();
				_applyNextLockTime = Time.get_timeSinceLevelLoad() + _lockOnTime / 2f;
				_aquiredLock = true;
				_lockStage = 1;
				SendLockOnNotification();
				crosshairContainer.lockTargetPosition = GetLockWorldPosition();
				crosshairContainer.lockStage = _lockStage;
			}
		}

		private void LoseLock()
		{
			_aquiredLock = (_isSwitchingLock = false);
			_lockStage = 0;
			_targetMachineId = -1;
			crosshairContainer.lockStage = _lockStage;
			SendLockOnNotification();
		}

		private void SendLockOnNotification(bool force = false)
		{
			if (force || _notifyTargetOfLock || _lockStage == 3 || _lockStage == 0)
			{
				LockOnNotifierDependency dependency = new LockOnNotifierDependency(_ownerPlayerId, _targetPlayerId, _lockStage, _lockedCube.gridPos, _ownerCurrentItemDescriptor.itemCategory, _ownerCurrentItemDescriptor.itemSize);
				_lockOnNotifier.Inject(dependency);
				_lockOnNotifier.Execute();
			}
			LockOnData lockOnData = new LockOnData(_ownerPlayerId, _targetPlayerId, _targetMachineId, _lockStage == 3, _lockedCube.gridPos);
			_lockOnStateObservable.Dispatch(ref lockOnData);
		}

		private void OnWeaponSwitched(int machineId, ItemDescriptor itemDescriptor)
		{
			if (_lockStage > 0)
			{
				LoseLock();
			}
			_currentSubCategory = itemDescriptor;
			_ownerCurrentItemDescriptor = itemDescriptor;
			if (machineId == _ownerMachineId)
			{
				_targeterIsActive = true;
				_initNeeded = true;
			}
			else
			{
				_targeterIsActive = false;
			}
		}

		private void InitData(ItemDescriptor itemDescriptor)
		{
			if (_weaponSubCategoryToNodes.TryGetValue(itemDescriptor, out LockOnTargetNode value))
			{
				ILockOnTargetingParametersComponent lockOn = value.lockOn;
				_maxRange = value.range.maxRange;
				_lockOnTime = lockOn.lockTime;
				_changeLockTime = lockOn.changeLockTime;
				_lockConeDot = lockOn.lockOnConeDot;
				_fullLockReleaseTime = lockOn.fullLockReleaseTime;
				_looseLock = lockOn.isLooseLock;
				_notifyTargetOfLock = lockOn.notifyTargetOfLock;
				_targetAllies = (_ownerCurrentItemDescriptor.itemCategory == ItemCategory.Nano);
				_initNeeded = false;
			}
		}

		private int MachineIdFromTransform(Transform transform)
		{
			GameObject machineBoard = GameUtility.GetMachineBoard(transform);
			return machineRootContainer.GetMachineIdFromRoot(TargetType.Player, machineBoard);
		}

		private bool TryGetCubeInLine(ref RaycastHit rcHit, ref Ray ray, int machineId, out InstantiatedCube cube)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			IMachineMap machineMap = machineManager.GetMachineMap(TargetType.Player, machineId);
			Rigidbody rigidBodyData = rigidbodyContainer.GetRigidBodyData(TargetType.Player, machineId);
			_dependency.Populate(rcHit.get_point(), rigidBodyData, ray.get_origin(), ray.get_direction(), _maxRange, machineMap, TargetType.Player, null);
			HitResult[] array = new HitResult[1];
			int cubeInGridStepLine = GridAllignedLineCheck.GetCubeInGridStepLine(_dependency, array);
			if (cubeInGridStepLine > 0)
			{
				cube = machineMap.GetCellAt(array[0].gridHit.hitGridPos).info;
				return true;
			}
			cube = null;
			return false;
		}

		private Vector3 GetLockWorldPosition()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			return _lockedRigidbody.get_position() + _lockedRigidbody.get_rotation() * GridScaleUtility.GridToWorld(_lockedCube.gridPos, TargetType.Player);
		}
	}
}
