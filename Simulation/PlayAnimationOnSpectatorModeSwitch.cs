using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal class PlayAnimationOnSpectatorModeSwitch : MonoBehaviour, IInitialize
	{
		[Inject]
		internal ISpectatorModeActivator spectatorModeActivator
		{
			private get;
			set;
		}

		public PlayAnimationOnSpectatorModeSwitch()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			if (spectatorModeActivator != null)
			{
				spectatorModeActivator.Register(PlayAnimation);
			}
		}

		private void OnDestroy()
		{
			if (spectatorModeActivator != null)
			{
				spectatorModeActivator.Unregister(PlayAnimation);
			}
		}

		private void PlayAnimation(int myKiller, bool enabled)
		{
			this.get_gameObject().SetActive(true);
			this.GetComponent<Animation>().Play();
		}
	}
}
