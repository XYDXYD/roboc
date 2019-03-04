using Simulation;
using Svelto.ES.Legacy;

internal interface IGUIDisplay : IComponent
{
	GuiScreens screenType
	{
		get;
	}

	TopBarStyle topBarStyle
	{
		get;
	}

	ShortCutMode shortCutMode
	{
		get;
	}

	bool isScreenBlurred
	{
		get;
	}

	bool hasBackground
	{
		get;
	}

	bool doesntHideOnSwitch
	{
		get;
	}

	HudStyle battleHudStyle
	{
		get;
	}

	GUIShowResult Show();

	bool Hide();

	void EnableBackground(bool enable);

	bool IsActive();
}
