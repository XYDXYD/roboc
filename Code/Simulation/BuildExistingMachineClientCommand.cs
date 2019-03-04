using Svelto.Command;

namespace Simulation
{
	internal sealed class BuildExistingMachineClientCommand : BuildMachineClientCommandBase, IInjectableCommand<BuildMachineCommandDependency>, ICommand
	{
		private BuildMachineCommandDependency _dependency;

		public ICommand Inject(BuildMachineCommandDependency dependency)
		{
			_dependency = dependency;
			return this;
		}

		public void Execute()
		{
			Build(_dependency.playerId, _dependency.teamId, _dependency.playerName, isAi: false, _dependency.spawnEffect, _dependency.deathEffect);
		}
	}
}
