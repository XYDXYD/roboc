using Svelto.IoC;
using UnityEngine;

internal sealed class HUDHider : MonoBehaviour
{
	private Camera _cameraComponent;

	[Inject]
	internal IGUIInputController inputController
	{
		private get;
		set;
	}

	[Inject]
	internal ChatPresenter chatPresenter
	{
		private get;
		set;
	}

	public HUDHider()
		: this()
	{
	}

	private void Start()
	{
		_cameraComponent = this.GetComponent<Camera>();
		inputController.OnScreenStateChange += HandleOnScreenStateChange;
	}

	private void Update()
	{
		if (InputRemapper.Instance.GetButtonDown("HideHud") && (chatPresenter == null || !chatPresenter.IsChatFocused()))
		{
			ToggleVisibility();
		}
	}

	private void HandleOnScreenStateChange()
	{
		_cameraComponent.set_enabled(true);
	}

	private void ToggleVisibility()
	{
		if (ShouldShowFullScreenHUD())
		{
			_cameraComponent.set_enabled(!_cameraComponent.get_enabled());
		}
	}

	private bool ShouldShowFullScreenHUD()
	{
		GuiScreens activeScreen = inputController.GetActiveScreen();
		if (!inputController.IsActiveScreenFullHUDStyle())
		{
			return false;
		}
		if (activeScreen == GuiScreens.SettingsScreen || activeScreen == GuiScreens.ControlsScreen)
		{
			return false;
		}
		return activeScreen != GuiScreens.PauseMenu;
	}

	private void OnDestroy()
	{
		inputController.OnScreenStateChange -= HandleOnScreenStateChange;
	}
}
