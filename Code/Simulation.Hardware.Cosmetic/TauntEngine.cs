using Fabric;
using RCNetwork.Events;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Taunts;
using UnityEngine;
using Utility;

namespace Simulation.Hardware.Cosmetic
{
	internal class TauntEngine : SingleEntityViewEngine<TaunterMachineNode>, IInitialize, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private const float BROADCAST_COOLDOWN_SECONDS = 30f;

		private TaunterMachineNode _localTaunter;

		private TauntDependency _tauntDependency = new TauntDependency();

		private float _nextAllowedTauntBroadcastTime;

		private TauntPressedObserver _tauntPressedObserver;

		private RemoteTauntObserver _remoteTauntObserver;

		private TauntsDeserialisedData _tauntData;

		private string _currentVfx;

		private Func<GameObject> _allocateTaunt;

		private GameObject _localTaunt;

		[Inject]
		private MachineSpawnDispatcher machineSpawnDispatcher
		{
			get;
			set;
		}

		[Inject]
		private ITauntMaskHelper tauntMaskHelper
		{
			get;
			set;
		}

		[Inject]
		private IServiceRequestFactory serviceFactory
		{
			get;
			set;
		}

		[Inject]
		private INetworkEventManagerClient _networkEventManager
		{
			get;
			set;
		}

		[Inject]
		private GameObjectPool _gameObjectPool
		{
			get;
			set;
		}

		[Inject]
		private IGameObjectFactory _gameObjectFactory
		{
			get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public unsafe TauntEngine(TauntPressedObserver tauntPressedObserver, RemoteTauntObserver remoteTauntObserver)
		{
			_tauntPressedObserver = tauntPressedObserver;
			_remoteTauntObserver = remoteTauntObserver;
			_tauntPressedObserver.AddAction((Action)HandlePlayerTaunt);
			_remoteTauntObserver.AddAction(new ObserverAction<TauntDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_allocateTaunt = OnTauntAllocation;
		}

		public void OnDependenciesInjected()
		{
			TauntsDeserialisedData data = null;
			IGetTauntsRequest getTauntsRequest = serviceFactory.Create<IGetTauntsRequest>();
			getTauntsRequest.SetAnswer(new ServiceAnswer<TauntsDeserialisedData>(delegate(TauntsDeserialisedData d)
			{
				data = d;
			}, delegate
			{
				Console.LogError("Unable to load taunts!");
			})).Execute();
			_tauntData = data;
			tauntMaskHelper.Initialise(data);
			machineSpawnDispatcher.OnPlayerRegistered += DetectMasks;
		}

		public void OnFrameworkInitialized()
		{
			foreach (KeyValuePair<string, string> activePrefabNamesForGroupName in _tauntData.ActivePrefabNamesForGroupNames)
			{
				_currentVfx = activePrefabNamesForGroupName.Value;
				_gameObjectPool.Preallocate(_currentVfx, 1, _allocateTaunt);
			}
		}

		public unsafe void OnFrameworkDestroyed()
		{
			machineSpawnDispatcher.OnPlayerRegistered -= DetectMasks;
			_tauntPressedObserver.RemoveAction((Action)HandlePlayerTaunt);
			_remoteTauntObserver.RemoveAction(new ObserverAction<TauntDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void DetectMasks(SpawnInParametersPlayer spawnInParameters)
		{
			if (spawnInParameters.isMe)
			{
				PreloadedMachine preloadedMachine = spawnInParameters.preloadedMachine;
				FasterList<InstantiatedCube> allInstantiatedCubes = preloadedMachine.machineMap.GetAllInstantiatedCubes();
				for (int num = allInstantiatedCubes.get_Count() - 1; num >= 0; num--)
				{
					InstantiatedCube instantiatedCube = allInstantiatedCubes.get_Item(num);
					tauntMaskHelper.CubePlaced(instantiatedCube.gridPos, instantiatedCube.persistentCubeData.cubeType.ID, (byte)instantiatedCube.rotationIndex, null);
				}
			}
		}

		protected override void Add(TaunterMachineNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				_localTaunter = node;
			}
		}

		protected override void Remove(TaunterMachineNode node)
		{
			if (node == _localTaunter)
			{
				_localTaunter = null;
			}
		}

		private void HandlePlayerTaunt()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			if ((!(_localTaunt != null) || !_localTaunt.get_activeSelf()) && tauntMaskHelper.GetRandomActivationInfo(out string groupName, out Vector3 effectAnchorLocation, out MaskOrientation effectAnchorOrientation))
			{
				effectAnchorLocation = tauntMaskHelper.CalculateRelativeMachineMaskOffset(groupName, effectAnchorLocation, effectAnchorOrientation.ToQuaternion());
				_tauntDependency.tauntId = groupName;
				_tauntDependency.relativePosition = effectAnchorLocation;
				_tauntDependency.relativeOrientation = effectAnchorOrientation.ToQuaternion();
				_tauntDependency.machineId = _localTaunter.ownerComponent.ownerMachineId;
				_localTaunt = TauntLocally(_localTaunter, _tauntDependency.tauntId, _tauntDependency.relativePosition, _tauntDependency.relativeOrientation);
				float timeSinceLevelLoad = Time.get_timeSinceLevelLoad();
				if (timeSinceLevelLoad > _nextAllowedTauntBroadcastTime)
				{
					_networkEventManager.SendEventToServer(NetworkEvent.Taunt, _tauntDependency);
					_nextAllowedTauntBroadcastTime = timeSinceLevelLoad + 30f;
				}
			}
		}

		private void HandleRemoteTaunt(ref TauntDependency parameter)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			TaunterMachineNode taunter = default(TaunterMachineNode);
			if (entityViewsDB.TryQueryEntityView<TaunterMachineNode>(parameter.machineId, ref taunter))
			{
				TauntLocally(taunter, parameter.tauntId, parameter.relativePosition, parameter.relativeOrientation);
			}
		}

		private GameObject TauntLocally(TaunterMachineNode taunter, string tauntId, Vector3 relativePos, Quaternion relativeRotation)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			_currentVfx = _tauntData.ActivePrefabNamesForGroupNames[tauntId];
			GameObject val = _gameObjectPool.Use(_currentVfx, _allocateTaunt);
			Transform transform = val.get_transform();
			transform.set_parent(taunter.rbComponent.rb.get_transform());
			transform.set_localPosition(relativePos);
			transform.set_localRotation(relativeRotation);
			transform.set_localScale(new Vector3(1f, 1f, 1f));
			val.SetActive(true);
			string text = _tauntData.SoundNamesForGroupNames[tauntId];
			EventManager.get_Instance().PostEvent(text, 0, (object)null, val);
			return val;
		}

		private GameObject OnTauntAllocation()
		{
			GameObject val = _gameObjectFactory.Build(_currentVfx);
			val.set_name(_currentVfx);
			return _gameObjectPool.AddRecycleOnDisableForGameObject(val, isPrefab: false);
		}

		public void Ready()
		{
		}
	}
}
