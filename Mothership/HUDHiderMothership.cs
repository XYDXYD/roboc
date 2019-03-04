using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class HUDHiderMothership : MonoBehaviour, IInitialize
	{
		private Camera _cameraComponent;

		[Inject]
		internal HUDHiderMothershipPresenter presenter
		{
			private get;
			set;
		}

		public HUDHiderMothership()
			: this()
		{
		}

		public void ToggleVisibility(bool setting)
		{
			_cameraComponent.set_enabled(setting);
		}

		public void OnDependenciesInjected()
		{
			UICamera val = Object.FindObjectOfType<UICamera>();
			_cameraComponent = val.GetComponent<Camera>();
			presenter.SetView(this);
		}
	}
}
