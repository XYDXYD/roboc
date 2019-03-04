using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Network
{
	internal sealed class UpdateGameModeSettingsCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private UpdateGameModeSettingsDependency _dependency;

		[Inject]
		public RespawnHealthSettingsObserver respawnHealthSettings
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as UpdateGameModeSettingsDependency);
			return this;
		}

		public void Execute()
		{
			respawnHealthSettings.ApplyRespawnHealSettings(_dependency.RespawnHealDuration, _dependency.RespawnFullHealDuration);
		}
	}
}
