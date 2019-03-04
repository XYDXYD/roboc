using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation
{
	internal class RespawnHealEffects : IInitialize, IWaitForFrameworkDestruction
	{
		private RespawnHealEffectData _respawnHealEffectData;

		private float _healDuration = 10f;

		private const string _animatorSwitchName = "FinishLooping";

		[Inject]
		internal MachineSpawnDispatcher machineDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal GameObjectPool shieldBubblePool
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
		internal LivePlayersContainer livePlayersContainer
		{
			private get;
			set;
		}

		[Inject]
		internal MachinePreloader machinePreloader
		{
			private get;
			set;
		}

		[Inject]
		internal RespawnHealthSettingsObserver settingsObserver
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			machineDispatcher.OnPlayerRespawnedIn += HandleOnSpawnedIn;
			settingsObserver.OnRespawnSettingsReceived += OnSettingsReceived;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			machineDispatcher.OnPlayerRespawnedIn -= HandleOnSpawnedIn;
			settingsObserver.OnRespawnSettingsReceived -= OnSettingsReceived;
		}

		private void OnSettingsReceived(float respawnHealDuration, float respawnFullHealDuration)
		{
			_healDuration = respawnHealDuration;
		}

		private void HandleOnSpawnedIn(SpawnInParametersPlayer spawnInParameters)
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			int machineId = spawnInParameters.machineId;
			GameObject val = (!spawnInParameters.isOnMyTeam) ? shieldBubblePool.Use(_respawnHealEffectData.respawnHealEffectEnemyPrefab.get_name(), (Func<GameObject>)(() => shieldBubblePool.AddRecycleOnDisableForGameObject(_respawnHealEffectData.respawnHealEffectEnemyPrefab))) : shieldBubblePool.Use(_respawnHealEffectData.respawnHealEffectPrefab.get_name(), (Func<GameObject>)(() => shieldBubblePool.AddRecycleOnDisableForGameObject(_respawnHealEffectData.respawnHealEffectPrefab)));
			Transform transform = val.get_transform();
			Rigidbody rigidBodyData = rigidbodyContainer.GetRigidBodyData(TargetType.Player, machineId);
			MachineInfo machineInfo = machinePreloader.GetPreloadedMachine(machineId).machineInfo;
			transform.set_parent(rigidBodyData.get_transform());
			transform.set_localRotation(Quaternion.get_identity());
			transform.set_localPosition(machineInfo.MachineCenter);
			transform.set_localScale(Vector3.get_one() * machineInfo.MachineSize.get_magnitude());
			Animator component = val.GetComponent<Animator>();
			val.SetActive(true);
			component.SetBool("FinishLooping", false);
			TaskRunner.get_Instance().Run(WaitForEffectFinish(Time.get_timeSinceLevelLoad(), spawnInParameters.playerId, val, component));
		}

		private IEnumerator WaitForEffectFinish(float startTime, int playerId, GameObject go, Animator animatorComponent)
		{
			while (Time.get_timeSinceLevelLoad() - startTime < _healDuration && livePlayersContainer.IsPlayerAlive(TargetType.Player, playerId))
			{
				EventManager.get_Instance().SetParameter("RespawnShield_Loop", "Shield_Time", (Time.get_timeSinceLevelLoad() - startTime) / _healDuration, go);
				if (Time.get_timeSinceLevelLoad() - startTime > _healDuration - _respawnHealEffectData.endAnimationDuration)
				{
					animatorComponent.SetBool("FinishLooping", true);
				}
				yield return null;
			}
			go.SetActive(false);
		}

		public void RegisterEffectData(RespawnHealEffectData respawnHealEffectData)
		{
			_respawnHealEffectData = respawnHealEffectData;
			shieldBubblePool.Preallocate(_respawnHealEffectData.respawnHealEffectPrefab.get_name(), 10, (Func<GameObject>)(() => shieldBubblePool.AddRecycleOnDisableForGameObject(_respawnHealEffectData.respawnHealEffectPrefab)));
			shieldBubblePool.Preallocate(_respawnHealEffectData.respawnHealEffectEnemyPrefab.get_name(), 10, (Func<GameObject>)(() => shieldBubblePool.AddRecycleOnDisableForGameObject(_respawnHealEffectData.respawnHealEffectEnemyPrefab)));
		}
	}
}
