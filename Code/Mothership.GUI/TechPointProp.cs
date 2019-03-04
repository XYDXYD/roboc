using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI
{
	internal class TechPointProp : MonoBehaviour, IInitialize
	{
		private GameObject _camera;

		[Inject]
		internal TechPointsPresenter presenter
		{
			get;
			set;
		}

		public TechPointProp()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			presenter.SetProp(this);
		}

		public void Show()
		{
			_camera = Camera.get_main().get_gameObject();
			_camera.SetActive(false);
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
			_camera.SetActive(true);
		}
	}
}
