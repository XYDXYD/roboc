using Svelto.IoC;
using Svelto.Tasks;
using System.Collections;
using UnityEngine;

namespace Simulation
{
	internal sealed class MultiplayerLoadedAnimation : MonoBehaviour, IInitialize
	{
		public AnimationClip clip;

		public float animationDelay;

		[Inject]
		internal MachineSpawnDispatcher networkMachineDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		private WorldSwitching worldSwitch
		{
			get;
			set;
		}

		public MultiplayerLoadedAnimation()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			if (WorldSwitching.IsMultiplayer())
			{
				Register();
			}
		}

		private void OnDestroy()
		{
			if (WorldSwitching.IsMultiplayer())
			{
				Unregister();
			}
		}

		private void Register()
		{
			worldSwitch.OnWorldJustSwitched += OnWorldJustSwitched;
		}

		private void Unregister()
		{
			worldSwitch.OnWorldJustSwitched -= OnWorldJustSwitched;
		}

		private void OnWorldJustSwitched(WorldSwitchMode mode)
		{
			TaskRunner.get_Instance().Run(AnimateAfterDelay(animationDelay));
		}

		private IEnumerator AnimateAfterDelay(float delay)
		{
			while (true)
			{
				float num;
				delay = (num = delay - Time.get_deltaTime());
				if (!(num > 0f))
				{
					break;
				}
				yield return null;
			}
			PlayAnimation();
		}

		private void PlayAnimation()
		{
			if (this != null)
			{
				Animation component = this.GetComponent<Animation>();
				if (component != null && clip != null)
				{
					component.Play(clip.get_name());
				}
			}
		}
	}
}
