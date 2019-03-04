using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal class EnableOnSpectatorMode : MonoBehaviour, IInitialize, IHudElement
	{
		[Inject]
		internal ISpectatorModeActivator spectatorModeActivator
		{
			private get;
			set;
		}

		[Inject]
		internal IHudStyleController battleHudStyleController
		{
			private get;
			set;
		}

		public EnableOnSpectatorMode()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			if (spectatorModeActivator != null)
			{
				spectatorModeActivator.Register(SetEnabled);
			}
		}

		private void Start()
		{
			battleHudStyleController.AddHud(this);
		}

		private void OnDestroy()
		{
			if (spectatorModeActivator != null)
			{
				spectatorModeActivator.Unregister(SetEnabled);
			}
			battleHudStyleController.RemoveHud(this);
		}

		private void SetEnabled(int myKiller, bool enabled)
		{
			this.get_gameObject().SetActive(enabled);
		}

		public void SetStyle(HudStyle style)
		{
			if (style == HudStyle.HideAllButChat)
			{
				this.get_gameObject().SetActive(false);
			}
		}
	}
}
