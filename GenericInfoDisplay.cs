using Mothership;
using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;

internal class GenericInfoDisplay : IGUIDisplay, IComponent
{
	private GenericInfoDialogue _dialog;

	private GenericErrorData _data;

	public GuiScreens screenType => GuiScreens.Info;

	public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

	public HudStyle battleHudStyle => HudStyle.Full;

	public ShortCutMode shortCutMode => ShortCutMode.NoKeyboardInputAllowed;

	public bool isScreenBlurred => true;

	public bool doesntHideOnSwitch => false;

	public bool hasBackground => false;

	[Inject]
	internal IGUIInputController guiController
	{
		private get;
		set;
	}

	public GUIShowResult Show()
	{
		if (_data != null)
		{
			_dialog.Open(_data);
			return GUIShowResult.Showed;
		}
		return GUIShowResult.NotShowed;
	}

	public bool Hide()
	{
		_dialog.Close();
		return true;
	}

	public bool IsActive()
	{
		if (_dialog == null)
		{
			return false;
		}
		return _dialog.isOpen;
	}

	public void EnableBackground(bool enable)
	{
	}

	internal void RegisterDialog(GenericInfoDialogue dialog)
	{
		_dialog = dialog;
	}

	public void ShowInfoDialogue(GenericErrorData data)
	{
		_data = data;
		guiController.ShowScreen(GuiScreens.Info);
	}
}
