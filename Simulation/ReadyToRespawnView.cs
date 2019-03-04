using Fabric;
using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal class ReadyToRespawnView : MonoBehaviour, IInitialize
	{
		public Animation labelAnimation;

		public string audioToPlay = "GUI_ClickToRespawn";

		[Inject]
		internal ReadyToRespawnPresenter presenter
		{
			private get;
			set;
		}

		public ReadyToRespawnView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			presenter.Register(this);
		}

		private void Update()
		{
			if (Input.get_anyKeyDown())
			{
				presenter.AnyKeyDown();
			}
		}

		private void OnEnable()
		{
			labelAnimation.Play();
			EventManager.get_Instance().PostEvent(audioToPlay, 0);
		}

		private void OnDisable()
		{
			labelAnimation.Stop();
			EventManager.get_Instance().PostEvent(audioToPlay, 1);
		}
	}
}
