using Simulation;
using UnityEngine;

internal sealed class AltimeterHud : MonoBehaviour
{
	public UILabel speedText;

	public string stringFormat;

	public float unitMultiplier = 1f;

	private AltimeterManagerBase _altimeterManager;

	private GameStartDispatcher _gameStartDispatcher;

	private UIPanel _panel;

	public AltimeterHud()
		: this()
	{
	}

	private void Start()
	{
		this.get_gameObject().SetActive(false);
	}

	internal void SetAltimeterManager(AltimeterManagerBase altimeterManager, GameStartDispatcher gameStartDispatcher)
	{
		_altimeterManager = altimeterManager;
		_gameStartDispatcher = gameStartDispatcher;
		gameStartDispatcher.Register(RegisterPopulate);
	}

	private void RegisterPopulate()
	{
		_altimeterManager.Register(Populate, UpdateEnabled);
	}

	private void OnDestroy()
	{
		if (_gameStartDispatcher != null)
		{
			_gameStartDispatcher.Unregister(RegisterPopulate);
		}
		if (_altimeterManager != null)
		{
			_altimeterManager.Unregister(Populate, UpdateEnabled);
		}
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
