using Fabric;
using Mothership;
using RoboCraft.MiniJSON;
using Simulation;
using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

internal sealed class EnterCodeDisplay : IGUIDisplay, IComponent
{
	private EnterCodeScreen _enterCodeScreen;

	public GuiScreens screenType => GuiScreens.EnterCodeScreen;

	public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

	public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

	public bool isScreenBlurred => true;

	public bool hasBackground => false;

	public bool doesntHideOnSwitch => false;

	public HudStyle battleHudStyle => HudStyle.Full;

	[Inject]
	public IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	[Inject]
	public SerialKeyManager serialKeyManager
	{
		private get;
		set;
	}

	[Inject]
	public PromoAwardedGuiFlow promoAwardedGuiFlow
	{
		private get;
		set;
	}

	[Inject]
	public IGUIInputControllerMothership guiInputController
	{
		private get;
		set;
	}

	public event Action OnShowEnterCodeScreen = delegate
	{
	};

	public void EnableBackground(bool enable)
	{
	}

	public void SetView(EnterCodeScreen view)
	{
		_enterCodeScreen = view;
	}

	public GUIShowResult Show()
	{
		_enterCodeScreen.Show();
		this.OnShowEnterCodeScreen();
		return GUIShowResult.Showed;
	}

	public bool Hide()
	{
		_enterCodeScreen.Hide();
		return true;
	}

	public bool IsActive()
	{
		if (_enterCodeScreen == null)
		{
			return false;
		}
		return _enterCodeScreen.IsActive();
	}

	public void ProcessCode(string code)
	{
		TaskRunner.get_Instance().Run(serialKeyManager.ApplySerialOrPromo(code, OnSuccess));
		if (UICamera.get_selectedObject() != null)
		{
			UIInput component = UICamera.get_selectedObject().GetComponent<UIInput>();
			if (component != null)
			{
				component.set_isSelected(false);
				UICamera.set_selectedObject(null);
			}
		}
	}

	private void OnSuccess(ApplyPromoCodeResponse response)
	{
		_enterCodeScreen.Confirm(delegate
		{
			guiInputController.CloseCurrentScreen();
			Dictionary<string, object> cubesAwarded = Json.Deserialize(response.CubesAwarded) as Dictionary<string, object>;
			TaskRunner.get_Instance().Run(promoAwardedGuiFlow.StartGuiFlowAsTask(cubesAwarded, response.CosmeticCreditsAwarded, response.RoboPassAwarded));
		}, response.MessageStrKey);
		EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_KubeMenuPurchased", 0, (object)null, _enterCodeScreen.get_gameObject());
	}
}
