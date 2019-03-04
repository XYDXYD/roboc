using Fabric;
using UnityEngine;

namespace Simulation
{
	internal class HeadLightSwitch : MonoBehaviour
	{
		public Light thelight;

		public Projector projector;

		public LensFlare lensFlare;

		private bool _initialized;

		private bool _enabled;

		public HeadLightSwitch()
			: this()
		{
		}

		private void Start()
		{
			_initialized = true;
			SetLight(lightState: true);
		}

		private void OnEnable()
		{
			if (_initialized && _enabled)
			{
				EnableLight(lightState: true);
			}
		}

		private void OnDisable()
		{
			if (_initialized && _enabled)
			{
				EnableLight(lightState: false);
			}
		}

		public void SetLight(bool lightState)
		{
			_enabled = lightState;
			EnableLight(lightState);
			if (this.get_gameObject() != null)
			{
				EventManager.get_Instance().PostEvent("KUB_kube_lights_toggle", 0, (object)null, this.get_gameObject());
			}
		}

		private void EnableLight(bool lightState)
		{
			if (thelight != null)
			{
				thelight.set_enabled(lightState);
			}
			if (projector != null)
			{
				projector.set_enabled(lightState);
			}
			if (lensFlare != null)
			{
				lensFlare.set_enabled(lightState);
			}
		}
	}
}
