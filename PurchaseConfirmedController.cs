using Simulation;
using Svelto.ES.Legacy;
using Svelto.Tasks;
using System;
using System.Collections;

internal class PurchaseConfirmedController : IGUIDisplay, IComponent
{
	private PurchaseConfirmedDisplay _purchaseConfirmedDisplay;

	public GuiScreens screenType => GuiScreens.ConfirmationDialog;

	public HudStyle battleHudStyle => HudStyle.Full;

	public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

	public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

	public bool doesntHideOnSwitch => false;

	public bool hasBackground => false;

	public bool isScreenBlurred => false;

	public void EnableBackground(bool enable)
	{
	}

	public bool IsActive()
	{
		if (_purchaseConfirmedDisplay == null)
		{
			return false;
		}
		return _purchaseConfirmedDisplay.IsActive();
	}

	public GUIShowResult Show()
	{
		_purchaseConfirmedDisplay.Show();
		return GUIShowResult.Showed;
	}

	public bool Hide()
	{
		_purchaseConfirmedDisplay.Hide();
		return true;
	}

	public void SetView(PurchaseConfirmedDisplay purchaseConfirmedDisplay)
	{
		_purchaseConfirmedDisplay = purchaseConfirmedDisplay;
	}

	public void ShowPremiumPurchased(int timeLeftDays)
	{
		_purchaseConfirmedDisplay.SetForPremiumTimeLimited(timeLeftDays);
		Show();
	}

	public void ShowLifeTimePremiumPurchased()
	{
		_purchaseConfirmedDisplay.SetForLifeTimePremium();
		Show();
	}

	public void ShowRoboPassPurchased()
	{
		TaskRunner.get_Instance().Run((Func<IEnumerator>)ShowRoboPassPurchasedEnumerator);
	}

	public void ShowCosmeticCreditsPurchased(int totalCCpurchased)
	{
		_purchaseConfirmedDisplay.SetForCosmeticCredits(totalCCpurchased);
		Show();
	}

	private IEnumerator ShowRoboPassPurchasedEnumerator()
	{
		yield return _purchaseConfirmedDisplay.SetForRoboPass();
		Show();
	}
}
