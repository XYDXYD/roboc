using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;

internal sealed class SettingsDisplay : IGUIDisplay, IComponent
{
	private SettingsScreen _settingsScreen;

	[Inject]
	public IPauseManager pauseManager
	{
		private get;
		set;
	}

	public GuiScreens screenType => GuiScreens.SettingsScreen;

	public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

	public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

	public bool isScreenBlurred => true;

	public bool hasBackground => false;

	public bool doesntHideOnSwitch => false;

	public HudStyle battleHudStyle => HudStyle.HideAll;

	public event Action OnShowSettingsScreen = delegate
	{
	};

	public void EnableBackground(bool enable)
	{
	}

	public void SetScreen(SettingsScreen settingsScreen)
	{
		_settingsScreen = settingsScreen;
	}

	public GUIShowResult Show()
	{
		_settingsScreen.Show();
		this.OnShowSettingsScreen();
		pauseManager.Pause(pause: true);
		return GUIShowResult.Showed;
	}

	public bool Hide()
	{
		_settingsScreen.Hide();
		pauseManager.Pause(pause: false);
		return true;
	}

	public bool IsActive()
	{
		if (_settingsScreen == null)
		{
			return false;
		}
		return _settingsScreen.IsActive();
	}
}
