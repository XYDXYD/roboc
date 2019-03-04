using Mothership;
using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.ServiceLayer;

internal class TopBarDisplay : IGUIDisplay, ITopBarDisplay, IComponent
{
	protected TopBar _topBar;

	protected TopBarBuildMode _topBarBuildMode;

	[Inject]
	internal IGUIInputControllerMothership guiInputController
	{
		private get;
		set;
	}

	[Inject]
	internal LoadingIconPresenter loadingIconPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	public GuiScreens screenType => GuiScreens.TopBar;

	public TopBarStyle topBarStyle => TopBarStyle.Default;

	public ShortCutMode shortCutMode => ShortCutMode.AllShortCuts;

	public bool isScreenBlurred => true;

	public bool hasBackground => false;

	public bool doesntHideOnSwitch => false;

	public HudStyle battleHudStyle => HudStyle.Full;

	public void EnableBackground(bool enable)
	{
	}

	public virtual GUIShowResult Show()
	{
		_topBar.Show();
		return GUIShowResult.Showed;
	}

	public void AddSelfToScreensList()
	{
		guiInputController.AddDisplayScreens(new IGUIDisplay[1]
		{
			this
		});
	}

	public void SetTopBarBuildMode(TopBarBuildMode topBarBuildMode)
	{
		_topBarBuildMode = topBarBuildMode;
		topBarBuildMode.Build();
	}

	public TopBar GetTopBar()
	{
		return _topBar;
	}

	public void SetTopBar(TopBar topbar)
	{
		_topBar = topbar;
		topbar.Build();
	}

	public bool Hide()
	{
		_topBar.Hide();
		return true;
	}

	public bool IsActive()
	{
		if (_topBar == null)
		{
			return false;
		}
		return _topBar.IsActive();
	}

	public void SetDisplayStyle(TopBarStyle topBarStyle)
	{
		if (topBarStyle == TopBarStyle.BuildMode)
		{
			_topBarBuildMode.Show();
			return;
		}
		_topBarBuildMode.Hide();
		_topBar.SetDisplayStyle(topBarStyle);
	}
}
