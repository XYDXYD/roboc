using Simulation;
using UnityEngine;

internal sealed class SpeedometerHud : MonoBehaviour
{
	public UILabel speedText;

	public string stringFormat;

	public float unitMultiplier = 1f;

	private SpeedometerManagerBase _speedometerManager;

	private GameStartDispatcher _gameStartDispatcher;

	private UIPanel _panel;

	public SpeedometerHud()
		: this()
	{
	}

	private void Start()
	{
		this.get_gameObject().SetActive(false);
	}

	internal void SetSpeedometerManager(SpeedometerManagerBase speedometerManager, GameStartDispatcher gameStartDispatcher)
	{
		_speedometerManager = speedometerManager;
		_gameStartDispatcher = gameStartDispatcher;
		gameStartDispatcher.Register(RegisterPopulate);
	}

	private void RegisterPopulate()
	{
		_speedometerManager.Register(Populate, UpdateEnabled);
	}

	private void OnDestroy()
	{
		_gameStartDispatcher.Unregister(RegisterPopulate);
		_speedometerManager.Unregister(Populate, UpdateEnabled);
	}

	private void Populate(float speed)
	{
		speed *= unitMultiplier;
		speedText.set_text(speed.ToString(stringFormat));
	}

	private void UpdateEnabled(bool enabled)
	{
		this.get_gameObject().SetActive(enabled);
	}
}
