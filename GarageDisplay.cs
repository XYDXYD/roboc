using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;

internal sealed class GarageDisplay : IGUIDisplay, IComponent
{
	[Inject]
	internal GaragePresenter garage
	{
		private get;
		set;
	}

	public GuiScreens screenType => GuiScreens.Garage;

	public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

	public ShortCutMode shortCutMode => ShortCutMode.AllShortCuts;

	public bool isScreenBlurred => false;

	public bool hasBackground => true;

	public bool doesntHideOnSwitch => false;

	public HudStyle battleHudStyle => HudStyle.Full;

	public event Action OnShowGarage = delegate
	{
	};

	public event Action OnHideGarage = delegate
	{
	};

	public void EnableBackground(bool enable)
	{
	}

	public GUIShowResult Show()
	{
		GUIShowResult gUIShowResult = garage.Show();
		if (gUIShowResult == GUIShowResult.Showed)
		{
			this.OnShowGarage();
		}
		return gUIShowResult;
	}

	public bool Hide()
	{
		bool flag = garage.Hide();
		if (flag)
		{
			this.OnHideGarage();
		}
		return flag;
	}

	public bool IsActive()
	{
		if (garage == null)
		{
			return false;
		}
		return garage.IsActive();
	}
}
