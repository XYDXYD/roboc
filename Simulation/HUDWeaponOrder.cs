using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class HUDWeaponOrder : MonoBehaviour, IInitialize
	{
		public UIGrid cubeListGrid;

		public WeaponOrderButton[] buttons;

		private bool _spectatorModeActive;

		private HudStyle _battleHudStyle;

		[Inject]
		internal WeaponOrderPresenter weaponOrderPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerSim guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public ISpectatorModeActivator spectatorModeActivator
		{
			private get;
			set;
		}

		public HUDWeaponOrder()
			: this()
		{
		}

		private void Start()
		{
			guiInputController.OnScreenStateChange += HandleOnScreenStateChange;
			spectatorModeActivator.Register(HandleOnSpectatorModeActivated);
		}

		private void HandleOnSpectatorModeActivated(int arg1, bool activated)
		{
			if (activated)
			{
				weaponOrderPresenter.ResetAllAlpha();
			}
			this.get_gameObject().SetActive(!activated && _battleHudStyle != HudStyle.HideAllButChat);
			_spectatorModeActive = activated;
		}

		private void HandleOnScreenStateChange()
		{
			if (guiInputController.GetActiveScreen() == GuiScreens.PauseMenu || _battleHudStyle == HudStyle.HideAllButChat)
			{
				this.get_gameObject().SetActive(false);
			}
			else if (!_spectatorModeActive)
			{
				this.get_gameObject().SetActive(true);
			}
		}

		void IInitialize.OnDependenciesInjected()
		{
			this.get_gameObject().SetActive(false);
			weaponOrderPresenter.SetView(this);
		}

		public void SetActive(bool active)
		{
			this.get_gameObject().SetActive(active && _battleHudStyle != HudStyle.HideAllButChat);
			cubeListGrid.Reposition();
		}

		public void SetStyle(HudStyle style)
		{
			_battleHudStyle = style;
		}
	}
}
