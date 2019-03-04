using Mothership;
using Svelto.IoC;
using UnityEngine;

internal sealed class CurrentGarageSlotNameWidget : MonoBehaviour, IInitialize
{
	private UILabel _label;

	[Inject]
	internal GaragePresenter garage
	{
		private get;
		set;
	}

	[Inject]
	internal RobotShopObserver shopObserver
	{
		private get;
		set;
	}

	public CurrentGarageSlotNameWidget()
		: this()
	{
	}

	public void Awake()
	{
		_label = this.GetComponent<UILabel>();
	}

	void IInitialize.OnDependenciesInjected()
	{
		if (garage != null)
		{
			garage.AddCurrentGarageNameChangedListener(OnGarageNameChange);
		}
		if (shopObserver != null)
		{
			shopObserver.OnRobotBuiltEvent += OnGarageNameChange;
			shopObserver.OnShowRobotEvent += OnShowRobot;
		}
	}

	private void OnDestroy()
	{
		if (shopObserver != null)
		{
			shopObserver.OnRobotBuiltEvent -= OnGarageNameChange;
			shopObserver.OnShowRobotEvent -= OnShowRobot;
		}
		if (garage != null)
		{
			garage.RemoveCurrentGarageNameChangedListener(OnGarageNameChange);
		}
	}

	private void OnShowRobot(RobotToShow robot)
	{
		OnGarageNameChange(robot.name);
	}

	private void OnGarageNameChange(string name)
	{
		_label.set_text(name);
	}
}
