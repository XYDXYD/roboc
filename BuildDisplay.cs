using PlayMaker;
using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;

internal sealed class BuildDisplay : IGUIDisplay, IPlaymakerCommandProvider, IPlaymakerDataProvider, IComponent
{
	[Inject]
	internal PremiumMembership premiumMembership
	{
		private get;
		set;
	}

	public GuiScreens screenType => GuiScreens.BuildMode;

	public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

	public ShortCutMode shortCutMode => ShortCutMode.BuildShortCuts;

	public bool isScreenBlurred => false;

	public bool hasBackground => true;

	public bool doesntHideOnSwitch => false;

	public HudStyle battleHudStyle => HudStyle.Full;

	public void EnableBackground(bool enable)
	{
	}

	public GUIShowResult Show()
	{
		return GUIShowResult.Showed;
	}

	public bool Hide()
	{
		return true;
	}

	public bool IsActive()
	{
		return false;
	}

	void IPlaymakerCommandProvider.RegisterPlaymakerCommandHandlers(Action<Action<IPlaymakerCommandInputParameters>, Type> RegisterPlayMakerCommandHandlerAction)
	{
	}

	void IPlaymakerDataProvider.RegisterPlaymakerRequestHandlers(Action<Type, Action<IPlayMakerDataRequest>> RegisterPlayMakerRequestHandler)
	{
	}

	private void HandleGetPlayerIsPremiumRequest(IPlayMakerDataRequest dataProvided)
	{
	}
}
