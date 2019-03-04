using Mothership;
using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;

internal sealed class PaintColorSelectorDisplay : IGUIDisplay, IComponent
{
	[Inject]
	public IGUIInputControllerMothership guiInputController
	{
		private get;
		set;
	}

	[Inject]
	public PaintToolPresenter paintToolPresenter
	{
		private get;
		set;
	}

	public GuiScreens screenType => GuiScreens.PaintColorSelector;

	public TopBarStyle topBarStyle => WorldSwitching.IsInBuildMode() ? TopBarStyle.OffScreen : TopBarStyle.FullScreenInterface;

	public ShortCutMode shortCutMode => (!WorldSwitching.IsInBuildMode()) ? ShortCutMode.OnlyGUINoSwitching : ShortCutMode.BuildShortCuts;

	public bool isScreenBlurred => true;

	public bool hasBackground => false;

	public bool doesntHideOnSwitch => false;

	public HudStyle battleHudStyle => HudStyle.Full;

	public void EnableBackground(bool enable)
	{
	}

	public GUIShowResult Show()
	{
		paintToolPresenter.SetHUDActive(active: true);
		return GUIShowResult.Showed;
	}

	public bool Hide()
	{
		paintToolPresenter.SetHUDActive(active: false);
		return true;
	}

	public bool IsActive()
	{
		if (paintToolPresenter == null)
		{
			return false;
		}
		return paintToolPresenter.IsHUDActive();
	}

	public void NewColorSelected()
	{
		guiInputController.ShowScreen(GuiScreens.BuildMode);
	}
}
