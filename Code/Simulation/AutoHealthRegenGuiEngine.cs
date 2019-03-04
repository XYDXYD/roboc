using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.ECS;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation
{
	internal class AutoHealthRegenGuiEngine : SingleEntityViewEngine<AutoHealEntityView>, IEngine, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IInitialize, IEngine
	{
		private IHealthBarViewComponent _healthViewComponent;

		private bool _settingsLoaded;

		private bool _enableAutoHeal;

		private float _thresholdToStartSound = 1f;

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		[Inject]
		internal HealthTracker healthTracker
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
			TaskRunner.get_Instance().Run(Tick());
		}

		public void OnDependenciesInjected()
		{
			IGetAutoRegenHealthSettings getAutoRegenHealthSettings = serviceFactory.Create<IGetAutoRegenHealthSettings>();
			getAutoRegenHealthSettings.SetAnswer(new ServiceAnswer<AutoRegenHealthSettingsData>(OnSettingsLoaded, OnFailed));
			getAutoRegenHealthSettings.Execute();
			gameEndedObserver.OnGameEnded += OnGameEnded;
		}

		public void OnFrameworkDestroyed()
		{
			gameEndedObserver.OnGameEnded -= OnGameEnded;
		}

		private void OnSettingsLoaded(AutoRegenHealthSettingsData settingsData)
		{
			_thresholdToStartSound = settingsData.thresholdToStartSound;
			_enableAutoHeal = settingsData.enableAutoHeal;
			_settingsLoaded = true;
		}

		private void OnFailed(ServiceBehaviour behaviour)
		{
			ErrorWindow.ShowServiceErrorWindow(behaviour);
		}

		protected override void Add(AutoHealEntityView entityView)
		{
			if (entityView.ownerComponent.ownedByMe)
			{
				entityView.autoHealComponent.healCancelled.NotifyOnValueSet((Action<int, bool>)OnAutoHealCancelled);
			}
		}

		protected override void Remove(AutoHealEntityView entityView)
		{
			if (entityView.ownerComponent.ownedByMe)
			{
				entityView.autoHealComponent.healCancelled.StopNotify((Action<int, bool>)OnAutoHealCancelled);
			}
		}

		public Type[] AcceptedComponents()
		{
			return new Type[1]
			{
				typeof(IHealthBarViewComponent)
			};
		}

		public void Add(IComponent component)
		{
			if (component is IHealthBarViewComponent)
			{
				_healthViewComponent = (component as IHealthBarViewComponent);
			}
		}

		public void Remove(IComponent component)
		{
			if (component is IHealthBarViewComponent)
			{
				_healthViewComponent = null;
			}
		}

		private IEnumerator Tick()
		{
			while (!_settingsLoaded)
			{
				yield return null;
			}
			while (_enableAutoHeal)
			{
				int count = default(int);
				AutoHealGuiEntityView[] guiEntityViews = entityViewsDB.QueryEntityViewsAsArray<AutoHealGuiEntityView>(ref count);
				if (count != 0)
				{
					AutoHealGuiEntityView autoHealGuiEntityView = guiEntityViews[0];
					TickFeedbackForMachine(entityViewsDB.QueryEntityView<AutoHealEntityView>(autoHealGuiEntityView.get_ID()), autoHealGuiEntityView);
				}
				yield return true;
			}
		}

		private void TickFeedbackForMachine(AutoHealEntityView machineEntityView, AutoHealGuiEntityView guiEntityView)
		{
			IAutoHealComponent autoHealComponent = machineEntityView.autoHealComponent;
			IAutoHealGuiComponent autoHealGuiComponent = guiEntityView.autoHealGuiComponent;
			float deltaTime = Time.get_deltaTime();
			int ownerMachineId = machineEntityView.ownerComponent.ownerMachineId;
			bool flag = !healthTracker.IsFullHealth(TargetType.Player, ownerMachineId);
			if (flag)
			{
				float num = (float)Math.Ceiling(autoHealComponent.timer);
				autoHealGuiComponent.soundTimer -= deltaTime;
				if (autoHealGuiComponent.soundTimer <= 0f)
				{
					int num2 = (int)num;
					_healthViewComponent.SetTimerLabel(num2, enabled: true);
					if (num2 != autoHealGuiComponent.previousSecond && num > _thresholdToStartSound)
					{
						_healthViewComponent.PlayFirstAutoRegenAnimation();
					}
					autoHealGuiComponent.soundTimer = 1f;
					autoHealGuiComponent.previousSecond = num2;
				}
				if (autoHealComponent.spawnHealTimer <= 0f && autoHealComponent.timer > 0f && num <= _thresholdToStartSound)
				{
					int num3 = (int)num;
					if (!autoHealGuiComponent.animationPlayed && (float)num3 >= _thresholdToStartSound)
					{
						_healthViewComponent.PlaySecondAutoRegenAnimation();
						PlaySound(AudioFabricGameEvents.GUI_Autoregen_Countdown);
						autoHealGuiComponent.animationPlayed = true;
					}
				}
				else
				{
					if (!(autoHealComponent.spawnHealTimer > 0f) && !(autoHealComponent.timer <= 0f))
					{
						return;
					}
					_healthViewComponent.SetTimerLabel(0, enabled: false);
					if (!flag)
					{
						return;
					}
					if (autoHealGuiComponent.animationPlayed)
					{
						_healthViewComponent.StopSecondAutoRegenAnimation();
						StopSound(AudioFabricGameEvents.GUI_Autoregen_Countdown);
						autoHealGuiComponent.animationPlayed = false;
					}
					if (!autoHealGuiComponent.autoRegenSoundPlayed)
					{
						if (healthTracker.GetCurrentHealthPercent(TargetType.Player, ownerMachineId) * 100f < 97f)
						{
							PlaySound(AudioFabricGameEvents.GUI_AutoRegen);
						}
						else
						{
							PlaySound(AudioFabricGameEvents.GUI_AutoRegen2c);
						}
						autoHealGuiComponent.autoRegenSoundPlayed = true;
					}
				}
			}
			else
			{
				if (autoHealGuiComponent.animationPlayed)
				{
					_healthViewComponent.StopSecondAutoRegenAnimation();
					StopSound(AudioFabricGameEvents.GUI_Autoregen_Countdown);
					autoHealGuiComponent.animationPlayed = false;
				}
				if (autoHealGuiComponent.autoRegenSoundPlayed)
				{
					StopSound(AudioFabricGameEvents.GUI_AutoRegen);
					StopSound(AudioFabricGameEvents.GUI_AutoRegen2c);
					autoHealGuiComponent.autoRegenSoundPlayed = false;
				}
				_healthViewComponent.SetTimerLabel(0, enabled: false);
			}
		}

		private void OnAutoHealCancelled(int entityId, bool cancelled)
		{
			if (_enableAutoHeal && cancelled)
			{
				AutoHealEntityView autoHealEntityView = entityViewsDB.QueryEntityView<AutoHealEntityView>(entityId);
				AutoHealGuiEntityView autoHealGuiEntityView = entityViewsDB.QueryEntityView<AutoHealGuiEntityView>(entityId);
				IAutoHealGuiComponent autoHealGuiComponent = autoHealGuiEntityView.autoHealGuiComponent;
				IAutoHealComponent autoHealComponent = autoHealEntityView.autoHealComponent;
				if (autoHealGuiComponent.animationPlayed)
				{
					PlaySound(AudioFabricGameEvents.GUI_SelfHeal_Canceled);
					_healthViewComponent.StopSecondAutoRegenAnimation();
					StopSound(AudioFabricGameEvents.GUI_Autoregen_Countdown);
					autoHealGuiComponent.animationPlayed = false;
				}
				if (autoHealGuiComponent.autoRegenSoundPlayed)
				{
					StopSound(AudioFabricGameEvents.GUI_AutoRegen);
					StopSound(AudioFabricGameEvents.GUI_AutoRegen2c);
					autoHealGuiComponent.autoRegenSoundPlayed = false;
				}
			}
		}

		private void OnGameEnded(bool won)
		{
			_enableAutoHeal = false;
			StopSound(AudioFabricGameEvents.GUI_Autoregen_Countdown);
			StopSound(AudioFabricGameEvents.GUI_SelfHeal_Canceled);
			StopSound(AudioFabricGameEvents.GUI_AutoRegen);
			StopSound(AudioFabricGameEvents.GUI_AutoRegen2c);
		}

		private void PlaySound(AudioFabricGameEvents e)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[(int)e], 0);
		}

		private void StopSound(AudioFabricGameEvents e)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[(int)e], 1);
		}
	}
}
