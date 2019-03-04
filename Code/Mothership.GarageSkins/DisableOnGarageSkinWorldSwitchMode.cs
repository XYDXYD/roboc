using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Mothership.GarageSkins
{
	internal class DisableOnGarageSkinWorldSwitchMode : MonoBehaviour, IInitialize, IWaitForFrameworkDestruction
	{
		[SerializeField]
		private WorldSwitchMode worldModeToHide;

		[SerializeField]
		private bool garageBaySkinActivated;

		private bool _currentGarageSkinActive;

		private WorldSwitchMode _currentWorldMode = WorldSwitchMode.GarageMode;

		[Inject]
		private WorldSwitching worldSwitching
		{
			get;
			set;
		}

		[Inject]
		private GarageBaySkinNotificationObserver observer
		{
			get;
			set;
		}

		public DisableOnGarageSkinWorldSwitchMode()
			: this()
		{
		}

		public unsafe void OnDependenciesInjected()
		{
			observer.AddAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			worldSwitching.OnWorldJustSwitched += SetCurrentWorldMode;
		}

		public unsafe void OnFrameworkDestroyed()
		{
			observer.RemoveAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			worldSwitching.OnWorldJustSwitched -= SetCurrentWorldMode;
		}

		private void SetGarageBayActivated(ref bool garageSkinActivated)
		{
			_currentGarageSkinActive = garageSkinActivated;
			ToggleGO();
		}

		private void SetCurrentWorldMode(WorldSwitchMode currentSwitchMode)
		{
			_currentWorldMode = currentSwitchMode;
			ToggleGO();
		}

		private void ToggleGO()
		{
			bool flag = _currentGarageSkinActive == garageBaySkinActivated && _currentWorldMode == worldModeToHide;
			this.get_gameObject().SetActive(!flag);
		}
	}
}
