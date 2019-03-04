using Simulation;
using Svelto.ES.Legacy;
using UnityEngine;

internal class WorldSwitchLoadingDisplay : IGUIDisplay, IComponent
{
	public GuiScreens screenType => GuiScreens.WorldSwitchLoading;

	public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

	public ShortCutMode shortCutMode => ShortCutMode.NoKeyboardInputAllowed;

	public bool isScreenBlurred => false;

	public bool hasBackground => false;

	public bool doesntHideOnSwitch => false;

	public HudStyle battleHudStyle => HudStyle.Full;

	internal GameObject view
	{
		private get;
		set;
	}

	public GUIShowResult Show()
	{
		view.SetActive(true);
		return GUIShowResult.Showed;
	}

	public bool Hide()
	{
		Object.Destroy(view.get_gameObject());
		return true;
	}

	public void EnableBackground(bool enable)
	{
	}

	public bool IsActive()
	{
		return view != null && view.get_activeInHierarchy();
	}
}
