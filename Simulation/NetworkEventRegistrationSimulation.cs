using Commands.Client;
using Events.Dependencies;
using RCNetwork.Events;
using Simulation.BattleArena;
using Simulation.BattleArena.CapturePoint;
using Simulation.BattleArena.CapturePoints;
using Simulation.BattleArena.Equalizer;
using Simulation.Hardware.Cosmetic;
using Simulation.Hardware.Modules;
using Simulation.Hardware.Modules.PowerModule;
using Simulation.Network;
using Simulation.Pit;
using Simulation.TeamBuff;
using Svelto.Command;
using Svelto.Command.Dispatcher;

namespace Simulation
{
	internal abstract class NetworkEventRegistrationSimulation
	{
		private ICommandFactory _commandFactory;

		private IEventDispatcher _eventDispatcher;

		private INetworkEventManagerClient _networkEventManager;

		protected void RegisterEvents(ICommandFactory commandFactory, IEventDispatcher eventDispatcher, INetworkEventManagerClient networkEventManager)
		{
			_commandFactory = commandFactory;
			_eventDispatcher = eventDispatcher;
			_networkEventManager = networkEventManager;
			RegisterLocalEvents();
			RegisterLoadingEvents();
		}

		private void RegisterLocalEvents()
		{
			_eventDispatcher.Add<NetworkEvent>(NetworkEvent.OnDisconnectedFromServer, _commandFactory.Build<DisplayServerDisconnectionErrorCommand>());
			_eventDispatcher.Add<NetworkEvent>(NetworkEvent.OnDisconnectedFromServer, _commandFactory.Build<HandleNetworkErrorClientCommand>());
			_eventDispatcher.Add<NetworkEvent>(NetworkEvent.OnConnectedToServer, _commandFactory.Build<HandleConnectedToServerSimulationCommand>());
		}

		private void RegisterLoadingEvents()
		{
			_networkEventManager.RegisterEvent<PlayerIDsAndNamesDependency>(NetworkEvent.PlayerIDs, _commandFactory.Build<SetPlayerIDsClientCommand>());
			_networkEventManager.RegisterEvent<PlayerIDsDependency>(NetworkEvent.HostAIs, _commandFactory.Build<SetHostedAIsClientCommand>());
			_networkEventManager.RegisterEvent<NetworkDependency>(NetworkEvent.GameAborted, _commandFactory.Build<GameAbortedClientCommand>());
			_networkEventManager.RegisterEvent<StringCodeDependency>(NetworkEvent.WarnPlayer, _commandFactory.Build<DisplayGameServerWarningClientCommand>());
			_networkEventManager.RegisterEvent<LoadingProgressDependency>(NetworkEvent.BroadcastLoadingProgress, _commandFactory.Build<ReceiveLoadingProgressClientCommand>());
		}

		public static void RegisterSyncEvents(INetworkEventManagerClient _networkEventManager, ICommandFactory _commandFactory)
		{
			_networkEventManager.RegisterEvent(NetworkEvent.BeginSync, _commandFactory.Build<BeginSyncClientCommand>());
			switch (WorldSwitching.GetGameModeType())
			{
			case GameModeType.Normal:
				_networkEventManager.RegisterEvent<UpdateGameModeSettingsDependency>(NetworkEvent.GameModeSettings, _commandFactory.Build<UpdateGameModeSettingsCommand>());
				_networkEventManager.RegisterEvent<GetTeamBaseDependency>(NetworkEvent.TeamBase, _commandFactory.Build<SpawnTeamBaseClientCommand>());
				_networkEventManager.RegisterEvent<GetCapturePointsDependency>(NetworkEvent.RegisterCapturePoints, _commandFactory.Build<SpawnCapturePointsClientCommand>());
				_networkEventManager.RegisterEvent<GetEqualizerDependency>(NetworkEvent.RegisterEqualizer, _commandFactory.Build<SpawnEqualizerClientCommand>());
				_networkEventManager.RegisterEvent<FusionShieldStateDependency>(NetworkEvent.SetShieldState, _commandFactory.Build<SetFusionShieldStateClientCommand>());
				_networkEventManager.RegisterEvent<GameTimeDependency>(NetworkEvent.CurrentGameTime, _commandFactory.Build<UpdateCurrentGameTimeClientCommand>());
				_networkEventManager.RegisterEvent<HealedCubesDependency>(NetworkEvent.SyncTeamBaseCubes, _commandFactory.Build<SetNonWeaponHealedCubesClientCommand>());
				_networkEventManager.RegisterEvent<EqualizerNotificationDependency>(NetworkEvent.SyncEqualizerNotification, _commandFactory.Build<ReceiveEqualizerNotificationClientCommand>());
				break;
			case GameModeType.SuddenDeath:
				_networkEventManager.RegisterEvent<UpdateGameModeSettingsDependency>(NetworkEvent.GameModeSettings, _commandFactory.Build<UpdateGameModeSettingsCommand>());
				_networkEventManager.RegisterEvent<GameTimeDependency>(NetworkEvent.CurrentGameTime, _commandFactory.Build<UpdateCurrentGameTimeClientCommand>());
				break;
			case GameModeType.Pit:
				_networkEventManager.RegisterEvent<UpdateGameModeSettingsDependency>(NetworkEvent.GameModeSettings, _commandFactory.Build<UpdateGameModeSettingsCommand>());
				break;
			case GameModeType.TeamDeathmatch:
				_networkEventManager.RegisterEvent<UpdateTeamDeathmatchSettingsDependency>(NetworkEvent.GameModeSettings, _commandFactory.Build<UpdateTeamDeathMatchSettingsCommand>());
				_networkEventManager.RegisterEvent<GameTimeDependency>(NetworkEvent.CurrentGameTime, _commandFactory.Build<UpdateCurrentGameTimeClientCommand>());
				_networkEventManager.RegisterEvent<UpdateTeamDeathMatchDependency>(NetworkEvent.TeamDeathMatchState, _commandFactory.Build<UpdateTeamDeathMatchCommand>());
				break;
			}
			_networkEventManager.RegisterEvent<InitialiseGameStatsDependency>(NetworkEvent.InitialiseGameStats, _commandFactory.Build<InGameStatsInitialiseClientCommand>());
			_networkEventManager.RegisterEvent<SpawnPointDependency>(NetworkEvent.FreeSpawnPoint, _commandFactory.Build<SpawnAtPositionCommand>());
			_networkEventManager.RegisterEvent<SyncMachineCubesDependency>(NetworkEvent.SyncMachineCubes, _commandFactory.Build<ApplySyncMachineCubesClientCommand>());
			_networkEventManager.RegisterEvent(NetworkEvent.EndOfSync, _commandFactory.Build<ReceiveEndOfSyncClientCommand>());
		}

		public static void RegisterIngameEvents(INetworkEventManagerClient _networkEventManager, ICommandFactory _commandFactory)
		{
			switch (WorldSwitching.GetGameModeType())
			{
			case GameModeType.Normal:
				_networkEventManager.RegisterEvent<SpawnPointDependency>(NetworkEvent.FreeRespawnPoint, _commandFactory.Build<RespawnAtPositionClientCommand>());
				_networkEventManager.RegisterEvent<RespawnTimeDependency>(NetworkEvent.SetRespawnWaitingTime, _commandFactory.Build<ScheduleRespawnProcessCommand>());
				_networkEventManager.RegisterEvent<GameWonDependency>(NetworkEvent.GameWonBaseDestroyed, _commandFactory.Build<TeamWonBaseSupernovaClientCommand>());
				_networkEventManager.RegisterEvent<GameLostDependency>(NetworkEvent.GameLostBaseDestroyed, _commandFactory.Build<TeamLostBaseSupernovaClientCommand>());
				_networkEventManager.RegisterEvent<GameWonDependency>(NetworkEvent.GameWon, _commandFactory.Build<TriggerGameWonClientCommand>());
				_networkEventManager.RegisterEvent<GameLostDependency>(NetworkEvent.GameLost, _commandFactory.Build<TriggerGameLostClientCommand>());
				_networkEventManager.RegisterEvent<CurrentSurrenderVotesDependency>(NetworkEvent.SurrenderVoteStarted, _commandFactory.Build<SurrenderVoteStartedClientCommand>());
				_networkEventManager.RegisterEvent<CurrentSurrenderVotesDependency>(NetworkEvent.CurrentSurrenderVotes, _commandFactory.Build<UpdateCurrentSurrenderVotesClientCommand>());
				_networkEventManager.RegisterEvent<SurrenderDeclinedDependency>(NetworkEvent.SurrenderDeclined, _commandFactory.Build<SurrenderDeclinedClientCommand>());
				_networkEventManager.RegisterEvent<SurrenderTimesDependency>(NetworkEvent.SetSurrenderTimes, _commandFactory.Build<SurrenderTimesClientCommand>());
				_networkEventManager.RegisterEvent<ThreateningTheBaseCommandDependency>(NetworkEvent.PlayerThreateningBase, _commandFactory.Build<DefendTheBaseClientCommand>());
				_networkEventManager.RegisterEvent<PlayerIdDependency>(NetworkEvent.RemoteEnemySpotted, _commandFactory.Build<EnemySpottedReceivedClientCommand>());
				_networkEventManager.RegisterEvent<DamagedByEnemyShieldDependency>(NetworkEvent.DamagedByEnemyShield, _commandFactory.Build<DamagedByEnemyShieldClientCommand>());
				_networkEventManager.RegisterEvent<EqualizerNotificationDependency>(NetworkEvent.EqualizerNotification, _commandFactory.Build<ReceiveEqualizerNotificationClientCommand>());
				_networkEventManager.RegisterEvent<TeamBaseStateDependency>(NetworkEvent.CapturePointProgress, _commandFactory.Build<UpdateCapturePointProgressClientCommand>());
				_networkEventManager.RegisterEvent<CapturePointNotificationDependency>(NetworkEvent.CapturePointNotification, _commandFactory.Build<ReceiveCapturePointNotificationCommand>());
				_networkEventManager.RegisterEvent<TeamBuffDependency>(NetworkEvent.BuffTeamPlayers, _commandFactory.Build<BuffTeamPlayersClientCommand>());
				break;
			case GameModeType.SuddenDeath:
				_networkEventManager.RegisterEvent<ThreateningTheBaseCommandDependency>(NetworkEvent.PlayerThreateningBase, _commandFactory.Build<DefendTheBaseClientCommand>());
				_networkEventManager.RegisterEvent<PlayerIdDependency>(NetworkEvent.RemoteEnemySpotted, _commandFactory.Build<EnemySpottedReceivedClientCommand>());
				_networkEventManager.RegisterEvent<GameWonDependency>(NetworkEvent.GameWon, _commandFactory.Build<TriggerGameWonAlivePlayerClientCommand>());
				_networkEventManager.RegisterEvent<GameLostDependency>(NetworkEvent.GameLost, _commandFactory.Build<TriggerGameLostClientCommand>());
				_networkEventManager.RegisterEvent<GameTimeDependency>(NetworkEvent.CurrentGameTime, _commandFactory.Build<UpdateCurrentGameTimeClientCommand>());
				break;
			case GameModeType.Pit:
				_networkEventManager.RegisterEvent<GameTimeDependency>(NetworkEvent.CurrentGameTime, _commandFactory.Build<StartPitModeClientCommand>());
				_networkEventManager.RegisterEvent<SpawnPointDependency>(NetworkEvent.FreeRespawnPoint, _commandFactory.Build<RespawnAtPositionClientCommand>());
				_networkEventManager.RegisterEvent<UpdatePitScoreDependency>(NetworkEvent.PitLeaderBoardUpdate, _commandFactory.Build<UpdatePitScoreCommand>());
				_networkEventManager.RegisterEvent<GameWonDependency>(NetworkEvent.GameWon, _commandFactory.Build<TriggerPitGameWonClientCommand>());
				_networkEventManager.RegisterEvent<GameLostDependency>(NetworkEvent.GameLost, _commandFactory.Build<TriggerPitGameLostClientCommand>());
				_networkEventManager.RegisterEvent<RespawnTimeDependency>(NetworkEvent.SetRespawnWaitingTime, _commandFactory.Build<ScheduleRespawnProcessCommand>());
				_networkEventManager.RegisterEvent<PitModeStateDependency>(NetworkEvent.PitModeState, _commandFactory.Build<InitialisePitModeStateClientCommand>());
				break;
			case GameModeType.TeamDeathmatch:
				_networkEventManager.RegisterEvent<SpawnPointDependency>(NetworkEvent.FreeRespawnPoint, _commandFactory.Build<RespawnAtPositionClientCommand>());
				_networkEventManager.RegisterEvent<RespawnTimeDependency>(NetworkEvent.SetRespawnWaitingTime, _commandFactory.Build<ScheduleRespawnProcessCommand>());
				_networkEventManager.RegisterEvent<GameWonDependency>(NetworkEvent.GameWonBaseDestroyed, _commandFactory.Build<TriggerGameWonClientCommand>());
				_networkEventManager.RegisterEvent<GameLostDependency>(NetworkEvent.GameLostBaseDestroyed, _commandFactory.Build<TriggerGameLostClientCommand>());
				_networkEventManager.RegisterEvent<GameWonDependency>(NetworkEvent.GameWon, _commandFactory.Build<TriggerGameWonClientCommand>());
				_networkEventManager.RegisterEvent<GameLostDependency>(NetworkEvent.GameLost, _commandFactory.Build<TriggerGameLostClientCommand>());
				_networkEventManager.RegisterEvent<CurrentSurrenderVotesDependency>(NetworkEvent.SurrenderVoteStarted, _commandFactory.Build<SurrenderVoteStartedClientCommand>());
				_networkEventManager.RegisterEvent<CurrentSurrenderVotesDependency>(NetworkEvent.CurrentSurrenderVotes, _commandFactory.Build<UpdateCurrentSurrenderVotesClientCommand>());
				_networkEventManager.RegisterEvent<SurrenderDeclinedDependency>(NetworkEvent.SurrenderDeclined, _commandFactory.Build<SurrenderDeclinedClientCommand>());
				_networkEventManager.RegisterEvent<SurrenderTimesDependency>(NetworkEvent.SetSurrenderTimes, _commandFactory.Build<SurrenderTimesClientCommand>());
				_networkEventManager.RegisterEvent<PlayerIdDependency>(NetworkEvent.RemoteEnemySpotted, _commandFactory.Build<EnemySpottedReceivedClientCommand>());
				_networkEventManager.RegisterEvent<GameTimeDependency>(NetworkEvent.CurrentGameTime, _commandFactory.Build<UpdateCurrentGameTimeClientCommand>());
				_networkEventManager.RegisterEvent<TeamBuffDependency>(NetworkEvent.BuffTeamPlayers, _commandFactory.Build<BuffTeamPlayersClientCommand>());
				break;
			}
			_networkEventManager.RegisterEvent<SetFinalGameScoreDependency>(NetworkEvent.SetFinalGameScore, _commandFactory.Build<SetFinalGameScoreClientCommand>());
			_networkEventManager.RegisterEvent<UpdateGameStatsDependency>(NetworkEvent.UpdateGameStats, _commandFactory.Build<UpdateGameStatsClientCommand>());
			_networkEventManager.RegisterEvent<UpdateVotingAfterBattleDependency>(NetworkEvent.UpdateVotingAfterBattle, _commandFactory.Build<UpdateVotingAfterBattleClientCommand>());
			_networkEventManager.RegisterEvent<NetworkDependency>(NetworkEvent.BonusesFlushDone, _commandFactory.Build<BonusSavedClientCommand>());
			_networkEventManager.RegisterEvent<NetworkDependency>(NetworkEvent.SendBonus, _commandFactory.Build<SendBonusToGameServerCommand>());
			_networkEventManager.RegisterEvent<KillDependency>(NetworkEvent.MachineDestroyedConfirmed, _commandFactory.Build<MachineDestroyedConfirmedClientCommand>());
			_networkEventManager.RegisterEvent<MultiPlayerInputChangedDependency>(NetworkEvent.OnServerReceivedInputChange, _commandFactory.Build<ReceivePlayerInputClientCommand>());
			_networkEventManager.RegisterEvent<DestroyCubeDependency>(NetworkEvent.DestroyCubesFull, _commandFactory.Build<DestroyCubeClientCommand>());
			_networkEventManager.RegisterEvent<DestroyCubeEffectOnlyDependency>(NetworkEvent.DestroyCubeEffectOnly, _commandFactory.Build<DestroyCubeEffectOnlyClientCommand>());
			_networkEventManager.RegisterEvent<DestroyCubeNoEffectDependency>(NetworkEvent.DestroyCubeNoEffect, _commandFactory.Build<DestroyCubeNoEffectClientCommand>());
			_networkEventManager.RegisterEvent<WeaponFireEffectDependency>(NetworkEvent.FireWeaponEffect, _commandFactory.Build<PlayLocalWeaponFireEffectClientCommand>());
			_networkEventManager.RegisterEvent<FireMissDependency>(NetworkEvent.FireMiss, _commandFactory.Build<FireMissClientCommand>());
			_networkEventManager.RegisterEvent<MultipleFireMissesDependency>(NetworkEvent.MultipleFireMisses, _commandFactory.Build<MultipleFireMissesClientCommand>());
			_networkEventManager.RegisterEvent<GameTimeDependency>(NetworkEvent.TimeToGameStart, _commandFactory.Build<UpdateTimeToGameStartClientCommand>());
			_networkEventManager.RegisterEvent<GameStartDependency>(NetworkEvent.GameStarted, _commandFactory.Build<StartGameClientCommand>());
			_networkEventManager.RegisterEvent<GameEndDependency>(NetworkEvent.EndGame, _commandFactory.Build<TriggerGameEndClientCommand>());
			_networkEventManager.RegisterEvent<TeamBaseStateDependency>(NetworkEvent.TeamBaseState, _commandFactory.Build<UpdateTeamBaseProgressClientCommand>());
			_networkEventManager.RegisterEvent<TeamBaseStateDependency>(NetworkEvent.TeamBaseCaptureStart, _commandFactory.Build<OnTeamBaseCaptureStartedClientCommand>());
			_networkEventManager.RegisterEvent<TeamBaseStateDependency>(NetworkEvent.TeamBaseCaptureReset, _commandFactory.Build<OnTeamBaseCaptureResetClientCommand>());
			_networkEventManager.RegisterEvent<TeamBaseStateDependency>(NetworkEvent.TeamBaseCaptureStop, _commandFactory.Build<OnTeamBaseCaptureStoppedClientCommand>());
			_networkEventManager.RegisterEvent<TeamBaseStateDependency>(NetworkEvent.TeamBaseSectionComplete, _commandFactory.Build<OnTeamBaseSectionCompleteClientCommand>());
			_networkEventManager.RegisterEvent<TeamBaseStateDependency>(NetworkEvent.TeamBaseFinalSectionComplete, _commandFactory.Build<OnTeamBaseCaptureCompleteClientCommand>());
			_networkEventManager.RegisterEvent<TeamBaseStateDependency>(NetworkEvent.TeamBaseInitialise, _commandFactory.Build<InitialiseTeamBaseClientCommand>());
			_networkEventManager.RegisterEvent<RequestPingDependency>(NetworkEvent.GetClientPings, _commandFactory.Build<RespondToPingRequestClientCommand>());
			_networkEventManager.RegisterEvent<RequestPingDependency>(NetworkEvent.SetClientPing, _commandFactory.Build<SetClientPingClientCommand>());
			_networkEventManager.RegisterEvent<PlayerIdDependency>(NetworkEvent.AlignmentRectifierStarted, _commandFactory.Build<RemoteAlignmentRectifierClientcommand>());
			_networkEventManager.RegisterEvent<MapPingEventDependency>(NetworkEvent.MapPingEvent, _commandFactory.Build<MapPingEventClientCommand>());
			_networkEventManager.RegisterEvent<HealedCubesDependency>(NetworkEvent.HealSelfResponse, _commandFactory.Build<SetNonWeaponHealedCubesClientCommand>());
			_networkEventManager.RegisterEvent<KillDependency>(NetworkEvent.ConfirmedKill, _commandFactory.Build<KillConfirmedClientCommand>());
			_networkEventManager.RegisterEvent<KillDependency>(NetworkEvent.ConfirmedAssist, _commandFactory.Build<AssistConfirmedClientCommand>());
			_networkEventManager.RegisterEvent<LockOnNotifierDependency>(NetworkEvent.LockOnNotificationBroadcast, _commandFactory.Build<LockOnNotifierReceivedCommand>());
			_networkEventManager.RegisterEvent<ShieldModuleEventDependency>(NetworkEvent.SpawnShield, _commandFactory.Build<SpawnShieldClientCommand>());
			_networkEventManager.RegisterEvent<CloakModuleEventDependency>(NetworkEvent.MakeInvisible, _commandFactory.Build<MakePlayerInvisibleClientCommand>());
			_networkEventManager.RegisterEvent<CloakModuleEventDependency>(NetworkEvent.MakeVisible, _commandFactory.Build<MakePlayerVisibleClientCommand>());
			_networkEventManager.RegisterEvent<TeleportActivateEffectDependency>(NetworkEvent.ActivateTeleportEffect, _commandFactory.Build<ActivateTeleportEffectClientCommand>());
			_networkEventManager.RegisterEvent<SpawnEmpLocatorDependency>(NetworkEvent.SpawnEmpLocator, _commandFactory.Build<SpawnEmpLocatorClientCommand>());
			_networkEventManager.RegisterEvent<NetworkStunnedMachineEffectDependency>(NetworkEvent.SpawnEmpMachineEffect, _commandFactory.Build<SpawnStunMachineEffectClientCommand>());
			_networkEventManager.RegisterEvent<TauntDependency>(NetworkEvent.Taunt, _commandFactory.Build<SpawnRemoteTauntCommand>());
			_networkEventManager.RegisterEvent<SelectWeaponDependency>(NetworkEvent.BroadcastWeaponSelect, _commandFactory.Build<SetSelectedWeaponClientCommand>());
			_networkEventManager.RegisterEvent<HealedAllyCubesDependency>(NetworkEvent.HealAllyResponse, _commandFactory.Build<SetAllyCubesHealedClientCommand>());
			_networkEventManager.RegisterEvent<PlayerIdDependency>(NetworkEvent.EnergyModuleActivated, _commandFactory.Build<ActivatePowerModuleEffectClientCommand>());
			_networkEventManager.RegisterEvent<PlayerIdDependency>(NetworkEvent.RemoteRadarModuleActivated, _commandFactory.Build<RadarActivationReceivedClientCommand>());
			_networkEventManager.RegisterEvent(NetworkEvent.PlayerQuitRequestComplete, _commandFactory.Build<PlayerQuitRequestCompleteCommand>());
			_networkEventManager.RegisterEvent<PlayerIdDependency>(NetworkEvent.OnAnotherClientDisconnected, _commandFactory.Build<AnotherPlayerDisconnectedClientCommand>());
			_networkEventManager.RegisterEvent<PlayerIdDependency>(NetworkEvent.OnClientReconnected, _commandFactory.Build<PlayerReconnectedClientCommand>());
		}
	}
}
