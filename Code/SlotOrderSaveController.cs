using Mothership;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using Utility;

internal class SlotOrderSaveController : IInitialize, ITickable, ITickableBase
{
	private const float IDLE_TIME_TO_SAVE = 3f;

	private float _currentTimeToSave;

	private bool _hasSlotMoved;

	[Inject]
	public GarageSlotOrderPresenter slotOrderPresenter
	{
		private get;
		set;
	}

	[Inject]
	public IServiceRequestFactory serviceFactory
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

	void IInitialize.OnDependenciesInjected()
	{
		slotOrderPresenter.OnSlotsReordered += OnSlotsReordered;
		guiInputController.OnScreenStateChange += ScreenChange;
	}

	public void Tick(float deltaSec)
	{
		if (_hasSlotMoved)
		{
			_currentTimeToSave += deltaSec;
			if (_currentTimeToSave >= 3f)
			{
				SaveSlotOrder();
				_hasSlotMoved = false;
			}
		}
	}

	private void OnSlotsReordered()
	{
		_hasSlotMoved = true;
		_currentTimeToSave = 0f;
	}

	private void ScreenChange()
	{
		GuiScreens activeScreen = guiInputController.GetActiveScreen();
		if ((activeScreen == GuiScreens.Garage || activeScreen == GuiScreens.RobotShop || activeScreen == GuiScreens.PauseMenu || activeScreen == GuiScreens.PlayScreen || activeScreen == GuiScreens.CubeDepot) && _hasSlotMoved)
		{
			SaveSlotOrder();
			_hasSlotMoved = false;
		}
	}

	private void SaveSlotOrder()
	{
		FasterList<int> currentSlotOrder = slotOrderPresenter.currentSlotOrder;
		ISetGarageSlotOrderRequest setGarageSlotOrderRequest = serviceFactory.Create<ISetGarageSlotOrderRequest, SetGarageSlotOrderDependency>(new SetGarageSlotOrderDependency(currentSlotOrder));
		setGarageSlotOrderRequest.SetAnswer(new ServiceAnswer<SetGarageSlotOrderDependency>(OnGarageSlotOrderSaved, OnGarageSlotOrderSaveFailed));
		setGarageSlotOrderRequest.Execute();
	}

	private void OnGarageSlotOrderSaved(SetGarageSlotOrderDependency setGarageSlotOrderDependency)
	{
		Console.Log("Slot order saved");
	}

	private void OnGarageSlotOrderSaveFailed(ServiceBehaviour serviceBehaviour)
	{
		Console.Log("Slot order save failed");
	}
}
