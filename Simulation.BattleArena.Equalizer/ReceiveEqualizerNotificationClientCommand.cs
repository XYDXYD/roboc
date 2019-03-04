using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.BattleArena.Equalizer
{
	internal class ReceiveEqualizerNotificationClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private EqualizerNotificationDependency _eqNotificationDep;

		[Inject]
		internal EqualizerNotificationObservable observable
		{
			private get;
			set;
		}

		[Inject]
		internal HealthTracker healthTracker
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_eqNotificationDep = (dependency as EqualizerNotificationDependency);
			return this;
		}

		public void Execute()
		{
			if (_eqNotificationDep.EqualizerNotific == EqualizerNotification.Activate)
			{
				int myTeam = playerTeamsContainer.GetMyTeam();
				int teamID = _eqNotificationDep.TeamID;
				bool isOnMyTeam = myTeam == teamID;
				SpawnInParametersEntity spawnInParameters = new SpawnInParametersEntity(0, teamID, isOnMyTeam, TargetType.EqualizerCrystal, null, null);
				spawnDispatcher.EntityRespawnedIn(spawnInParameters);
				if (_eqNotificationDep.Health >= 0)
				{
					healthTracker.SetEqualizerHealthAfterReconnect(_eqNotificationDep.Health, _eqNotificationDep.MaxHealth);
				}
			}
			observable.Dispatch(ref _eqNotificationDep);
		}
	}
}
