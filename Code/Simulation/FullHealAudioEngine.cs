using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class FullHealAudioEngine : IInitialize
	{
		[Inject]
		internal HealthTracker healthTracker
		{
			private get;
			set;
		}

		[Inject]
		internal RigidbodyDataContainer rigidBodyContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachineContainer
		{
			private get;
			set;
		}

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			healthTracker.OnPlayerHealthChanged += HandleOnPlayerHealthChanged;
			gameObjectPool.Preallocate(1, 5, (Func<GameObject>)gameObjectPool.AddRecycleOnDisableForAudio);
		}

		private void HandleOnPlayerHealthChanged(HealthTracker.PlayerHealthChangedInfo info)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			if (info.deltaHealth > 0 && healthTracker.IsFullHealth(TargetType.Player, info.shotMachineId))
			{
				Rigidbody rigidBodyData = rigidBodyContainer.GetRigidBodyData(TargetType.Player, info.shotMachineId);
				GameObject val = CreateAudioGameObject();
				val.get_transform().set_parent(rigidBodyData.get_transform());
				val.get_transform().set_localPosition(rigidBodyData.get_centerOfMass());
				int num = 108;
				EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[num], 0, val);
			}
		}

		private GameObject CreateAudioGameObject()
		{
			GameObject val = gameObjectPool.Use(1, (Func<GameObject>)gameObjectPool.AddRecycleOnDisableForAudio);
			val.SetActive(true);
			return val;
		}
	}
}
