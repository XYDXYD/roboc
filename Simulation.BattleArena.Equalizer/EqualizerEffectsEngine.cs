using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Simulation.BattleArena.Equalizer
{
	internal class EqualizerEffectsEngine : SingleEntityViewEngine<EqualizerEffectsNode>, IInitialize
	{
		private EqualizerEffectsNode _node;

		private EqualizerNotificationObserver _equalizerNotificationObserver;

		[Inject]
		public HealthTracker healthTracker
		{
			private get;
			set;
		}

		public EqualizerEffectsEngine(EqualizerNotificationObserver observer)
		{
			_equalizerNotificationObserver = observer;
		}

		public void OnDependenciesInjected()
		{
			healthTracker.OnEntityHealthChanged += HandleOnEntityHealthChanged;
		}

		protected unsafe override void Add(EqualizerEffectsNode node)
		{
			_node = node;
			_equalizerNotificationObserver.AddAction(new ObserverAction<EqualizerNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected unsafe override void Remove(EqualizerEffectsNode node)
		{
			_node = null;
			_equalizerNotificationObserver.RemoveAction(new ObserverAction<EqualizerNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleOnEntityHealthChanged(TargetType type, int id, float currentPercent, float arg4)
		{
			if (type == TargetType.EqualizerCrystal)
			{
				float num = 1f - currentPercent;
				IMaterialComponent materialComponent = _node.materialComponent;
				materialComponent.material.SetFloat(materialComponent.healthPropertyId, num);
				materialComponent.material.SetFloat(materialComponent.crackPropertyId, num);
				float num2 = num * 10f - 0.5f;
				GameObject[] godRays = _node.godRaysComponent.godRays;
				for (int i = 0; i < godRays.Length; i++)
				{
					GameObject val = godRays[i];
					val.SetActive(num2 > (float)i);
				}
				IAudioComponent audioComponent = _node.audioComponent;
				EventManager.get_Instance().SetParameter(audioComponent.loopAudioEvent, audioComponent.loopAudioParameter, num, _node.rootComponent.root);
			}
		}

		private void OnNotificationReceived(ref EqualizerNotificationDependency parameter)
		{
			switch (parameter.EqualizerNotific)
			{
			case EqualizerNotification.ActivationWarning:
				break;
			case EqualizerNotification.Cancelled:
				break;
			case EqualizerNotification.Activate:
				ActivateCrystal(parameter);
				break;
			case EqualizerNotification.Deactivated:
			case EqualizerNotification.Defended:
				DeactivateCrystal(parameter);
				break;
			case EqualizerNotification.Destroyed:
				DestroyCrystal(parameter);
				break;
			}
		}

		private void DeactivateCrystal(EqualizerNotificationDependency parameter)
		{
			for (int i = 0; i < _node.godRaysComponent.godRays.Length; i++)
			{
				GameObject val = _node.godRaysComponent.godRays[i];
				val.SetActive(false);
			}
			EventManager.get_Instance().PostEvent(_node.audioComponent.loopAudioEvent, 1, (object)null, _node.rootComponent.root);
			EventManager.get_Instance().SetParameter(_node.audioComponent.loopAudioEvent, _node.audioComponent.loopAudioParameter, 0f, _node.rootComponent.root);
		}

		private void DestroyCrystal(EqualizerNotificationDependency parameter)
		{
			for (int i = 0; i < _node.godRaysComponent.godRays.Length; i++)
			{
				GameObject val = _node.godRaysComponent.godRays[i];
				val.SetActive(false);
			}
			EventManager.get_Instance().PostEvent(_node.audioComponent.loopAudioEvent, 1, (object)null, _node.rootComponent.root);
			EventManager.get_Instance().SetParameter(_node.audioComponent.loopAudioEvent, _node.audioComponent.loopAudioParameter, 0f, _node.rootComponent.root);
		}

		private void ActivateCrystal(EqualizerNotificationDependency parameter)
		{
			float num = 0f;
			if (parameter.Health > 0)
			{
				float num2 = (float)parameter.Health / (float)parameter.MaxHealth;
				num = 1f - num2;
			}
			IMaterialComponent materialComponent = _node.materialComponent;
			materialComponent.material.SetFloat(materialComponent.healthPropertyId, num);
			materialComponent.material.SetFloat(materialComponent.crackPropertyId, num);
			IAudioComponent audioComponent = _node.audioComponent;
			string loopAudioEvent = audioComponent.loopAudioEvent;
			GameObject root = _node.rootComponent.root;
			EventManager.get_Instance().PostEvent(loopAudioEvent, 0, (object)null, root);
			EventManager.get_Instance().SetParameter(loopAudioEvent, audioComponent.loopAudioParameter, 0f, root);
		}
	}
}
