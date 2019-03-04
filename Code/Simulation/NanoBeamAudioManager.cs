using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class NanoBeamAudioManager : IInitialize, IWaitForFrameworkDestruction
	{
		private string _soundOnRespawn;

		private GameObject _respawnEffect;

		private Transform _cameraTransform;

		[Inject]
		internal HealingReporter healingReporter
		{
			private get;
			set;
		}

		[Inject]
		internal GameObjectPool objectPool
		{
			private get;
			set;
		}

		[Inject]
		internal RigidbodyDataContainer rigidbodyDataContainer
		{
			private get;
			set;
		}

		public void Init(string audioCubeDestroyed, string audioCubeHealed, GameObject respawnEffectPrefab)
		{
			_soundOnRespawn = audioCubeHealed;
			_respawnEffect = respawnEffectPrefab;
			objectPool.Preallocate(_respawnEffect.get_name(), 5, (Func<GameObject>)(() => objectPool.AddRecycleOnDisableForParticlesGO(_respawnEffect)));
			_cameraTransform = Camera.get_main().get_transform();
		}

		void IInitialize.OnDependenciesInjected()
		{
			healingReporter.OnPlayerCubesRespawned += HandleOnPlayerCubesRespawned;
			objectPool.Preallocate(1, 5, (Func<GameObject>)(() => objectPool.AddRecycleOnDisableForAudio()));
		}

		private void HandleOnPlayerCubesRespawned(int shooterId, int machineId, FasterList<InstantiatedCube> respawnedCubes, TargetType shooterTargetType)
		{
			PlaySound(machineId, respawnedCubes.get_Item(0), _soundOnRespawn, TargetType.Player);
			PlayEffect(machineId, respawnedCubes, TargetType.Player);
		}

		private void PlayEffect(int hitMachineId, FasterList<InstantiatedCube> respawnedCubes, TargetType targetType)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, hitMachineId);
			for (int i = 0; i < 1; i++)
			{
				GameObject val = objectPool.Use(_respawnEffect.get_name(), (Func<GameObject>)(() => objectPool.AddRecycleOnDisableForParticlesGO(_respawnEffect)));
				val.SetActive(true);
				Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(rigidBodyData.get_transform(), respawnedCubes.get_Item(i).gridPos, targetType);
				val.get_transform().set_position(cubeWorldPosition);
				Vector3 val2 = _cameraTransform.get_position() - cubeWorldPosition;
				val.get_transform().set_rotation(Quaternion.LookRotation(val2));
			}
		}

		private void PlaySound(int hitMachineId, InstantiatedCube cube, string eventName, TargetType targetType)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitMachineId);
			Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(rigidBodyData.get_transform(), cube.gridPos, targetType);
			GameObject val = objectPool.Use(1, (Func<GameObject>)(() => objectPool.AddRecycleOnDisableForAudio()));
			val.SetActive(true);
			val.get_transform().set_position(cubeWorldPosition);
			EventManager.get_Instance().PostEvent(eventName, 0, (object)null, val);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			healingReporter.OnPlayerCubesRespawned -= HandleOnPlayerCubesRespawned;
		}
	}
}
