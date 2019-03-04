using Svelto.ECS;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class PowerBarEntity : MonoBehaviour, IPowerBarDataComponent, IRemoveEntityComponent
	{
		[SerializeField]
		private UISlider powerBar;

		[SerializeField]
		private Animation animation;

		[SerializeField]
		private string notEnoughPowerAnimationName = "Hud_Overclocker_NoFire";

		private float _powerValue;

		private bool _progressiveIncrementActive = true;

		float IPowerBarDataComponent.powerValue
		{
			get
			{
				return _powerValue;
			}
			set
			{
				_powerValue = value;
			}
		}

		float IPowerBarDataComponent.powerPercent
		{
			get
			{
				return powerBar.get_value();
			}
			set
			{
				powerBar.set_value(value);
			}
		}

		bool IPowerBarDataComponent.progressiveIncrementActive
		{
			get
			{
				return _progressiveIncrementActive;
			}
			set
			{
				_progressiveIncrementActive = value;
			}
		}

		[Inject]
		public ISpectatorModeActivator spectatorModeActivator
		{
			private get;
			set;
		}

		[Inject]
		public IEntityFactory enginesRoot
		{
			private get;
			set;
		}

		public Action removeEntity
		{
			get;
			set;
		}

		public PowerBarEntity()
			: this()
		{
		}

		private void OnDestroy()
		{
			if (removeEntity != null)
			{
				removeEntity();
			}
		}

		private void Start()
		{
			enginesRoot.BuildEntity<PowerBarEntityDescriptor>(this.GetInstanceID(), new object[1]
			{
				this
			});
			spectatorModeActivator.Register(HandleOnSpectatorModeActivated);
		}

		private void HandleOnSpectatorModeActivated(int arg1, bool activated)
		{
			this.get_gameObject().SetActive(!activated);
		}

		void IPowerBarDataComponent.PlayNotEnoughPowerAnimation()
		{
			animation.Play(notEnoughPowerAnimationName);
		}
	}
}
