using Svelto.Command;
using Svelto.Command.Dispatcher;

namespace Simulation
{
	internal sealed class StartPlanetSinglePlayerClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private WorldSwitching _worldSwitch;

		public void Execute()
		{
			_worldSwitch.StartPlanet();
		}

		public IDispatchableCommand Inject(object worldSwitch)
		{
			_worldSwitch = (worldSwitch as WorldSwitching);
			return this;
		}
	}
}
