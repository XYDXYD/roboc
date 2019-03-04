using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class CubeDestroyedAudio : IInitialize, IWaitForFrameworkDestruction
	{
		private Func<GameObject> _onAudioAllocation;

		[Inject]
		public DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		public PlayerMachinesContainer playerMachinesContainer
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

		[Inject]
		public RigidbodyDataContainer rigidBodyDataContainer
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
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnPlayerDamageApplied += HandleOnPlayerDamageApplied;
			destructionReporter.OnMachineKilled += HandlePlayerKilledBy;
			_onAudioAllocation = OnAudioAllocation;
			gameObjectPool.Preallocate(1, 50, _onAudioAllocation);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnPlayerDamageApplied -= HandleOnPlayerDamageApplied;
			destructionReporter.OnMachineKilled -= HandlePlayerKilledBy;
		}

		private void HandlePlayerKilledBy(int ownerId, int shooterId)
		{
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			if (ownerId != shooterId)
			{
				bool flag = playerTeamsContainer.IsMe(TargetType.Player, ownerId);
				bool flag2 = playerTeamsContainer.IsMe(TargetType.Player, shooterId);
				if (flag)
				{
					EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_PlayerDefeated", 0);
				}
				else
				{
					PlayBotDefeatedSound(ownerId);
				}
				if (flag || flag2)
				{
					int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, ownerId);
					IMachineMap machineMap = machineManager.GetMachineMap(TargetType.Player, activeMachine);
					HashSet<InstantiatedCube> remainingCubes = machineMap.GetRemainingCubes();
					Rigidbody rigidBodyData = rigidBodyDataContainer.GetRigidBodyData(TargetType.Player, activeMachine);
					GameObject val = CreateAudioGameObject();
					val.get_transform().set_parent(rigidBodyData.get_transform());
					val.get_transform().set_localPosition(rigidBodyData.get_centerOfMass());
					PlayCubesDestroyedAudio(remainingCubes.Count, val);
				}
			}
		}

		private void PlayBotDefeatedSound(int ownerId)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, ownerId);
			Rigidbody rigidBodyData = rigidBodyDataContainer.GetRigidBodyData(TargetType.Player, activeMachine);
			GameObject val = CreateAudioGameObject();
			val.get_transform().set_parent(rigidBodyData.get_transform());
			val.get_transform().set_localPosition(rigidBodyData.get_centerOfMass());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_BotDefeated", 0, val);
		}

		private GameObject OnAudioAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForAudio();
		}

		private void HandleOnPlayerDamageApplied(DestructionData data)
		{
			if ((data.targetIsMe || data.shooterIsMe) && !data.isDestroyed && data.destroyedCubes.get_Count() > 0)
			{
				GameObject destroyedCubeGameObject = GetDestroyedCubeGameObject(data.hitRigidbody, data.destroyedCubes.get_Item(0));
				PlayCubesDestroyedAudio(data.destroyedCubes.get_Count(), destroyedCubeGameObject);
			}
		}

		private GameObject GetDestroyedCubeGameObject(Rigidbody rb, InstantiatedCube destroyedCube)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(destroyedCube.gridPos, rb, TargetType.Player);
			GameObject val = CreateAudioGameObject();
			val.get_transform().set_position(cubeWorldPosition);
			return val;
		}

		private GameObject CreateAudioGameObject()
		{
			GameObject val = gameObjectPool.Use(1, _onAudioAllocation);
			val.SetActive(true);
			return val;
		}

		private void PlayCubesDestroyedAudio(float destroyedCubesCount, GameObject go)
		{
			string empty = string.Empty;
			empty = ((destroyedCubesCount <= 5f) ? AudioFabricEvent.Name(AudioFabricGameEvents.CubesDestroyed_1_5) : ((destroyedCubesCount <= 14f) ? AudioFabricEvent.Name(AudioFabricGameEvents.CubesDestroyed_6_14) : ((destroyedCubesCount <= 35f) ? AudioFabricEvent.Name(AudioFabricGameEvents.CubesDestroyed_15_35) : ((!(destroyedCubesCount <= 59f)) ? AudioFabricEvent.Name(AudioFabricGameEvents.CubesDestroyed_60) : AudioFabricEvent.Name(AudioFabricGameEvents.CubesDestroyed_36_59)))));
			EventManager.get_Instance().PostEvent(empty, 0, go);
		}
	}
}
