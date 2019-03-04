using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class RemoteAlignmentRectifierManager : IInitialize, IWaitForFrameworkDestruction
	{
		private class RemoteAlignmentRectifier
		{
			private GameObject _remotePlayerMachineRigidbodyGobj;

			private GameObjectPool _gameobjectPool;

			private AlignmentRectifierEffectView _alignmentRectifierEffectView;

			private float _boundingSphereRadius;

			private Vector3 _boundingSphereLocalOffset;

			public bool isPlaying
			{
				get;
				private set;
			}

			public RemoteAlignmentRectifier(Rigidbody rb, MachineInfo machineInfo, GameObjectPool gameobjectPool)
			{
				_remotePlayerMachineRigidbodyGobj = rb.get_gameObject();
				_gameobjectPool = gameobjectPool;
				ComputeBoundingSphere(machineInfo);
			}

			public void StartAlignmentRectifierEffect(AlignmentRectifierData alignmentRectifierData)
			{
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				GameObject val = _gameobjectPool.Use(alignmentRectifierData.particleEffectTemplate.get_name(), (Func<GameObject>)(() => GameObjectPool.CreateGameObjectFromPrefab(alignmentRectifierData.particleEffectTemplate)));
				val.SetActive(true);
				_alignmentRectifierEffectView = val.GetComponent<AlignmentRectifierEffectView>();
				val.get_transform().set_parent(_remotePlayerMachineRigidbodyGobj.get_transform());
				val.get_transform().set_localRotation(Quaternion.get_identity());
				val.get_transform().set_localPosition(_boundingSphereLocalOffset);
				_alignmentRectifierEffectView.SetSize(_boundingSphereRadius);
				_alignmentRectifierEffectView.Play(_gameobjectPool, alignmentRectifierData);
				isPlaying = true;
				TaskRunner.get_Instance().Run(TimeOut());
			}

			private IEnumerator TimeOut()
			{
				yield return (object)new WaitForSecondsEnumerator(3f);
				HandleAlignmentRectifierTimerEnded();
			}

			public void StopAlignmentRectifierEffect()
			{
				_alignmentRectifierEffectView.Stop();
			}

			private void HandleAlignmentRectifierTimerEnded()
			{
				isPlaying = false;
			}

			private void ComputeBoundingSphere(MachineInfo machineInfo)
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				_boundingSphereRadius = machineInfo.MachineSize.get_magnitude() * 0.5f;
				_boundingSphereLocalOffset = machineInfo.MachineCenter;
			}
		}

		private AlignmentRectifierData _alignmentRectifierData;

		private Dictionary<int, RemoteAlignmentRectifier> _remoteAlignmentRectifiers = new Dictionary<int, RemoteAlignmentRectifier>();

		[Inject]
		internal GameObjectPool gameobjectPool
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
		internal MachineSpawnDispatcher machineSpawnDispatcher
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
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			machineSpawnDispatcher.OnPlayerSpawnedIn += HandleMachineSpawned;
			destructionReporter.OnMachineDestroyed += HandleOnMachineDestroyed;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			machineSpawnDispatcher.OnPlayerSpawnedIn -= HandleMachineSpawned;
			destructionReporter.OnMachineDestroyed -= HandleOnMachineDestroyed;
			_remoteAlignmentRectifiers.Clear();
		}

		public void UnregisterEffects(AlignmentRectifierData alignmentRectifierData)
		{
			_alignmentRectifierData = null;
		}

		public void RegisterEffects(AlignmentRectifierData alignmentRectifierData)
		{
			_alignmentRectifierData = alignmentRectifierData;
			gameobjectPool.Preallocate(_alignmentRectifierData.particleEffectTemplate.get_name(), 3, (Func<GameObject>)(() => GameObjectPool.CreateGameObjectFromPrefab(_alignmentRectifierData.particleEffectTemplate)));
		}

		public void StartAlignmentEffect(int playerId)
		{
			int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerId);
			if (_remoteAlignmentRectifiers.TryGetValue(activeMachine, out RemoteAlignmentRectifier value))
			{
				value.StartAlignmentRectifierEffect(_alignmentRectifierData);
			}
		}

		private void HandleOnMachineDestroyed(int playerId, int machineId, bool isMe)
		{
			if (_remoteAlignmentRectifiers.TryGetValue(machineId, out RemoteAlignmentRectifier value) && value.isPlaying)
			{
				value.StopAlignmentRectifierEffect();
			}
		}

		private void HandleMachineSpawned(SpawnInParametersPlayer spawnInParameters)
		{
			if (!spawnInParameters.isMe)
			{
				_remoteAlignmentRectifiers[spawnInParameters.machineId] = new RemoteAlignmentRectifier(spawnInParameters.preloadedMachine.rbData, spawnInParameters.preloadedMachine.machineInfo, gameobjectPool);
			}
		}
	}
}
