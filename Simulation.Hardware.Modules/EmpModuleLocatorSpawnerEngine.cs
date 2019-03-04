using RCNetwork.Events;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class EmpModuleLocatorSpawnerEngine : SingleEntityViewEngine<EmpModuleActivationNode>, IQueryingEntityViewEngine, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IEngine
	{
		private Camera _mainCamera;

		private Vector3 _middleScreenPoint;

		private SpawnEmpLocatorDependency _spawnEmpLocatorDependency = new SpawnEmpLocatorDependency();

		private RemoteSpawnEmpLocatorObserver _remoteSpawnEmpLocatorObserver;

		[Inject]
		internal EmpTargetingLocatorFactory empTargetingLocatorFactory
		{
			private get;
			set;
		}

		[Inject]
		internal INetworkEventManagerClient networkEventManager
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe EmpModuleLocatorSpawnerEngine(RemoteSpawnEmpLocatorObserver observer)
		{
			_remoteSpawnEmpLocatorObserver = observer;
			_remoteSpawnEmpLocatorObserver.AddAction(new ObserverAction<RemoteLocatorData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Ready()
		{
		}

		public void OnFrameworkInitialized()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			_mainCamera = Camera.get_main();
			_middleScreenPoint = new Vector3((float)(Screen.get_width() / 2), (float)(Screen.get_height() / 2), 1f);
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_remoteSpawnEmpLocatorObserver.RemoveAction(new ObserverAction<RemoteLocatorData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(EmpModuleActivationNode node)
		{
			node.activationComponent.activate.subscribers += HandleEmpModuleActivation;
			empTargetingLocatorFactory.PreallocateTargetingLocator("EMP_Intro_Beams", 1);
			empTargetingLocatorFactory.PreallocateTargetingLocator("EMP_Intro_Beams_E", 1);
		}

		protected override void Remove(EmpModuleActivationNode node)
		{
			node.activationComponent.activate.subscribers -= HandleEmpModuleActivation;
		}

		private void HandleEmpModuleActivation(IModuleActivationComponent sender, int moduleId)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			Ray val = _mainCamera.ScreenPointToRay(_middleScreenPoint);
			EmpModuleActivationNode empModuleActivationNode = default(EmpModuleActivationNode);
			if (entityViewsDB.TryQueryEntityView<EmpModuleActivationNode>(moduleId, ref empModuleActivationNode))
			{
				empModuleActivationNode.confirmActivationComponent.activationConfirmed.Dispatch(ref moduleId);
				RaycastHit val2 = default(RaycastHit);
				Vector3 val3 = (!Physics.Raycast(val, ref val2, empModuleActivationNode.rangeComponent.moduleRange, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK)) ? (_mainCamera.ScreenToWorldPoint(_middleScreenPoint) + _mainCamera.get_transform().get_forward() * empModuleActivationNode.rangeComponent.moduleRange) : val2.get_point();
				empTargetingLocatorFactory.Build("EMP_Intro_Beams", val3, empModuleActivationNode, isOnMyTeam: true);
				_spawnEmpLocatorDependency.Inject(val3, empModuleActivationNode.stunRadiusComponent.stunRadius, empModuleActivationNode.countdownComponent.countdown, empModuleActivationNode.durationComponent.stunDuration, empModuleActivationNode.ownerComponent.ownerId, empModuleActivationNode.ownerComponent.machineId);
				networkEventManager.SendEventToServer(NetworkEvent.BroadcastSpawnEmpLocator, _spawnEmpLocatorDependency);
			}
		}

		private void HandleSpawnRemoteLocator(ref RemoteLocatorData data)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<EmpModuleActivationNode> enumerator = entityViewsDB.QueryEntityViews<EmpModuleActivationNode>().GetEnumerator();
			EmpModuleActivationNode current;
			do
			{
				if (enumerator.MoveNext())
				{
					current = enumerator.get_Current();
					continue;
				}
				return;
			}
			while (current.ownerComponent.ownerId != data.ownerId);
			bool flag = !current.ownerComponent.isEnemy;
			if (flag)
			{
				empTargetingLocatorFactory.Build("EMP_Intro_Beams", data.position, current, flag);
			}
			else
			{
				empTargetingLocatorFactory.Build("EMP_Intro_Beams_E", data.position, current, flag);
			}
		}
	}
}
