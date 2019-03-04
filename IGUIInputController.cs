using Mothership;
using System;

internal interface IGUIInputController
{
	bool isInMainScreen
	{
		get;
	}

	event Action OnScreenStateChange;

	IGUIDisplay GetControllerForScreen(GuiScreens screen);

	GuiScreens GetActiveScreen();

	ShortCutMode GetShortCutMode();

	bool IsActiveScreenFullHUDStyle();

	void ShowScreen(GuiScreens newScreen);

	void ToggleScreen(GuiScreens newScreen);

	void CloseCurrentScreen(bool hideTopBar = true);

	void ToggleCurrentScreen();

	void ToggleScreenViaShortcut(GuiScreens screen);

	void SetShortCutMode(ShortCutMode shortCutMode);

	void AddDisplayScreens(IGUIDisplay[] displays);

	void UpdateShortCutMode();

	bool ShouldShowSocialGUI();

	bool IsTutorialScreenActive();

	void ForceCloseJustThisScreen(GuiScreens screen);

	void HandleQuitPressed();

	void AddFloatingWidget(IFloatingWidget w);

	void RemoveFloatingWidget(IFloatingWidget w);
}
