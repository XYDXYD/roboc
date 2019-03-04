using Svelto.Tasks;
using System;

internal interface IDispatchWorldSwitching
{
	ParallelTaskCollection OnWorldIsSwitching
	{
		get;
	}

	WorldSwitchMode CurrentWorld
	{
		get;
	}

	WorldSwitchMode SwitchingTo
	{
		get;
	}

	WorldSwitchMode SwitchingFrom
	{
		get;
	}

	bool SwitchingToSimulation
	{
		get;
	}

	bool SwitchingFromSimulation
	{
		get;
	}

	event Action<WorldSwitchMode> OnWorldJustSwitched;
}
