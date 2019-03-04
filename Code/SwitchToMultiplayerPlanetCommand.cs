using Battle;
using Svelto.Command;
using Svelto.IoC;

internal sealed class SwitchToMultiplayerPlanetCommand : IInjectableCommand<SwitchWorldDependency>, ICommand
{
	private SwitchWorldDependency _dependency;

	[Inject]
	internal WorldSwitching worldSwitching
	{
		private get;
		set;
	}

	[Inject]
	internal BattleTimer battleTimer
	{
		private get;
		set;
	}

	public ICommand Inject(SwitchWorldDependency dependency)
	{
		_dependency = dependency;
		return this;
	}

	public void Execute()
	{
		worldSwitching.SwitchToPlanetMultiplayer(_dependency);
		battleTimer.GameInitialised();
	}
}
