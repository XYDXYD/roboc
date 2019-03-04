using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Network
{
	internal sealed class UpdateTeamDeathMatchSettingsCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private UpdateTeamDeathmatchSettingsDependency _dependency;

		[Inject]
		public TeamDeathMatchStatsPresenter statsPresenter
		{
			private get;
			set;
		}

		[Inject]
		public RespawnHealthSettingsObserver respawnHealthSettings
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (UpdateTeamDeathmatchSettingsDependency)dependency;
			return this;
		}

		public void Execute()
		{
			GameModeSettings settings = _dependency.settings;
			int? killLimit = settings.killLimit;
			statsPresenter.OnSetKillLimit(killLimit.Value);
			respawnHealthSettings.ApplyRespawnHealSettings(settings.respawnHealDuration, settings.respawnFullHealDuration);
		}
	}
}
