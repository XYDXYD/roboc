using Svelto.Context;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class UIToggleOnWorldSwitch : MonoBehaviour, IInitialize, IWaitForFrameworkDestruction
	{
		[SerializeField]
		private GameObject gameObjectToToggle;

		[SerializeField]
		private WorldSwitchMode modeToShow;

		[Inject]
		private WorldSwitching worldSwitching
		{
			get;
			set;
		}

		public UIToggleOnWorldSwitch()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			worldSwitching.OnWorldJustSwitched += ToggleGameObject;
		}

		public void OnFrameworkDestroyed()
		{
			worldSwitching.OnWorldJustSwitched -= ToggleGameObject;
		}

		private void ToggleGameObject(WorldSwitchMode currentMode)
		{
			gameObjectToToggle.SetActive(currentMode == modeToShow);
		}
	}
}
