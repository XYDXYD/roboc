using Svelto.IoC;

internal sealed class SimulationTutorialScreen : TutorialScreenBase, IInitialize
{
	private bool _isActive;

	[Inject]
	internal ITutorialController tutorialController
	{
		private get;
		set;
	}

	[Inject]
	internal PremiumMembership premiumMembership
	{
		private get;
		set;
	}

	public override GuiScreens QueryScreenType()
	{
		return GuiScreens.SimulationTutorialScreen;
	}

	void IInitialize.OnDependenciesInjected()
	{
		tutorialController.SetDisplay(this);
	}

	public void Start()
	{
		this.get_gameObject().SetActive(false);
	}

	public bool IsActive()
	{
		return _isActive;
	}

	public void Show()
	{
		this.get_gameObject().SetActive(true);
		ShowScreenBase();
		_isActive = true;
	}

	public void HideScreen()
	{
		HideScreenBase();
		this.get_gameObject().SetActive(false);
		_isActive = false;
	}
}
