using Mothership.GUI.Inventory;
using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;

namespace Mothership
{
	internal sealed class CubeSelectorDisplay : IGUIDisplay, IComponent
	{
		[Inject]
		internal CubeSelectorPresenter presenter
		{
			private get;
			set;
		}

		public GuiScreens screenType => GuiScreens.InventoryScreen;

		public TopBarStyle topBarStyle => (!WorldSwitching.IsInBuildMode()) ? TopBarStyle.FullScreenInterface : TopBarStyle.BuildMode;

		public ShortCutMode shortCutMode => (!WorldSwitching.IsInBuildMode()) ? ShortCutMode.OnlyGUINoSwitching : ShortCutMode.BuildShortCuts;

		public bool isScreenBlurred => WorldSwitching.IsInBuildMode();

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.Full;

		public event Action OnShowCubeSelector = delegate
		{
		};

		public void EnableBackground(bool enable)
		{
		}

		public GUIShowResult Show()
		{
			if (WorldSwitching.IsInBuildMode())
			{
				presenter.SetFullSize();
			}
			else
			{
				presenter.SetHalfSize();
			}
			presenter.Show();
			this.OnShowCubeSelector();
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			presenter.Hide();
			presenter.SetFullSize();
			return true;
		}

		public bool IsActive()
		{
			if (presenter == null)
			{
				return false;
			}
			return presenter.IsActive();
		}
	}
}
