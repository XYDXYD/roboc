using Battle;
using UnityEngine;
using Utility;

internal class LobbyProgressBar : MonoBehaviour
{
	public UISprite progressBar;

	private int _numMachinesToLoad;

	private int _numMachinesLoaded;

	public float progressBarFullScale = 1f;

	internal MachinePreloader _machinePreloader
	{
		private get;
		set;
	}

	public LobbyProgressBar()
		: this()
	{
	}

	internal void Initialise(MachinePreloader machinePreloader, BattlePlayers battlePlayers)
	{
		_machinePreloader = machinePreloader;
		_machinePreloader.OnEachMachinePreloaded += OnMachinePreloaded;
		_numMachinesToLoad = battlePlayers.GetExpectedPlayers().Count;
		_numMachinesLoaded = 0;
		UpdateProgressBarScale();
	}

	internal void Fill()
	{
		_numMachinesLoaded = _numMachinesToLoad;
		UpdateProgressBarScale();
	}

	private void OnMachinePreloaded()
	{
		_numMachinesLoaded++;
		Console.Log("MACHINE BUILT (" + _numMachinesLoaded + "/" + _numMachinesToLoad + ")");
		UpdateProgressBarScale();
	}

	private void UpdateProgressBarScale()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localScale = progressBar.get_transform().get_localScale();
		localScale.x = (float)_numMachinesLoaded / (float)_numMachinesToLoad;
		localScale.x = Mathf.Clamp(localScale.x, 0.001f, progressBarFullScale);
		progressBar.get_transform().set_localScale(localScale);
	}

	private void OnDestroy()
	{
		if (_machinePreloader != null)
		{
			_machinePreloader.OnEachMachinePreloaded -= OnMachinePreloaded;
		}
	}
}
