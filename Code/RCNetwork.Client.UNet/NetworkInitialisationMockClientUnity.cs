using RCNetwork.Events;
using RCNetwork.Server;
using Simulation;
using Simulation.Network;
using Simulation.SinglePlayer;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace RCNetwork.Client.UNet
{
	internal sealed class NetworkInitialisationMockClientUnity
	{
		private readonly bool _isTestMode;

		private readonly IContainer _container;

		private IEventDispatcher _eventDispatcher;

		private NetworkEventManagerClientMock _networkEventManager;

		private INetworkEventManagerServer _networkEventManagerServer;

		private ICommandFactory _commandFactory;

		public NetworkInitialisationMockClientUnity(IContainer container, bool isTestMode)
		{
			_isTestMode = isTestMode;
			_container = container;
		}

		public void Initialize()
		{
			InitialiseFramework();
			InitialiseGame();
		}

		public void Start(int localPlayerId)
		{
			PlayerIdDependency playerIdDependency = new PlayerIdDependency(localPlayerId);
			_networkEventManagerServer.SendEventToPlayer(NetworkEvent.OnConnectedToGameServer, localPlayerId, playerIdDependency);
			_networkEventManagerServer.ReceiveEvent(NetworkEvent.OnPlayerConnectedToServer, localPlayerId, playerIdDependency.Serialise());
			_eventDispatcher.Dispatch<NetworkEvent>(NetworkEvent.OnConnectedToServer);
		}

		private void InitialiseFramework()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			_eventDispatcher = new EventDispatcher();
			_commandFactory = _container.Build<ICommandFactory>();
			_networkEventManagerServer = _container.Build<INetworkEventManagerServer>();
			_networkEventManager = (_container.Build<INetworkEventManagerClient>() as NetworkEventManagerClientMock);
			_networkEventManager.networkEventManagerServer = _networkEventManagerServer;
		}

		private void InitialiseGame()
		{
			RegisterRemoteEvents();
		}

		private void RegisterRemoteEvents()
		{
			_networkEventManagerServer.RegisterEvent<KillDependency>(NetworkEvent.MachineDestroyed, _commandFactory.Build<ScheduleRespawnProcessMockCommand>());
			_networkEventManagerServer.RegisterEvent<NetworkDependency>(NetworkEvent.RequestRespawnPoint, _commandFactory.Build<OnPlayerReadyToRespawnMockCommand>());
			_networkEventManager.RegisterEvent<SpawnPointDependency>(NetworkEvent.FreeRespawnPoint, _commandFactory.Build<RespawnAtPositionClientCommand>());
			_networkEventManager.RegisterEvent<SpawnPointDependency>(NetworkEvent.FreeSpawnPoint, _commandFactory.Build<SpawnAtPositionCommand>());
			_networkEventManager.RegisterEvent<UpdateTeamDeathMatchDependency>(NetworkEvent.TeamDeathMatchState, _commandFactory.Build<UpdateTeamDeathMatchCommand>());
			_networkEventManager.RegisterEvent<GameTimeDependency>(NetworkEvent.CurrentGameTime, _commandFactory.Build<UpdateCurrentGameTimeClientCommand>());
			_networkEventManager.RegisterEvent<GameWonDependency>(NetworkEvent.GameWon, _commandFactory.Build<TriggerGameWonClientCommand>());
			_networkEventManager.RegisterEvent<GameLostDependency>(NetworkEvent.GameLost, _commandFactory.Build<TriggerGameLostClientCommand>());
			_networkEventManager.RegisterEvent<UpdateGameStatsDependency>(NetworkEvent.UpdateGameStats, _commandFactory.Build<UpdateGameStatsClientCommand>());
			GameModeType gameModeType = WorldSwitching.GetGameModeType();
			if (WorldSwitching.IsTutorial())
			{
				_networkEventManagerServer.RegisterEvent<PlayerIdDependency>(NetworkEvent.OnPlayerConnectedToServer, _commandFactory.Build<GetFreeSpawnPointMockCommandTutorial>());
			}
			else if (_isTestMode)
			{
				_networkEventManagerServer.RegisterEvent<PlayerIdDependency>(NetworkEvent.OnPlayerConnectedToServer, _commandFactory.Build<GetFreeSpawnPointMockCommandTestMode>());
			}
			else if (gameModeType == GameModeType.Campaign)
			{
				UpdateCampaignSettingsCommand command = _commandFactory.Build<UpdateCampaignSettingsCommand>();
				_networkEventManager.RegisterEvent<UpdateGameModeSettingsDependency>(NetworkEvent.GameModeSettings, command);
				_networkEventManagerServer.RegisterEvent<PlayerIdDependency>(NetworkEvent.OnPlayerConnectedToServer, _commandFactory.Build<GetFreeSpawnPointMockCommandCampaign>());
			}
			else
			{
				UpdateTeamDeathMatchSettingsCommand command2 = _commandFactory.Build<UpdateTeamDeathMatchSettingsCommand>();
				_networkEventManager.RegisterEvent<UpdateTeamDeathmatchSettingsDependency>(NetworkEvent.GameModeSettings, command2);
				_networkEventManagerServer.RegisterEvent<PlayerIdDependency>(NetworkEvent.OnPlayerConnectedToServer, _commandFactory.Build<GetFreeSpawnPointServerMockCommand>());
			}
			_networkEventManagerServer.RegisterEvent<DestroyCubeDependency>(NetworkEvent.DamageCube, _commandFactory.Build<SinglePlayerValidateWeaponFireServerMockCommand>());
			_networkEventManagerServer.RegisterEvent<DestroyCubeEffectOnlyDependency>(NetworkEvent.DamageCubeEffectOnly, _commandFactory.Build<SinglePlayerValidateWeaponFireEffectOnlyServerMockCommand>());
			_networkEventManagerServer.RegisterEvent<DestroyCubeNoEffectDependency>(NetworkEvent.DamageCubeNoEffect, _commandFactory.Build<SinglePlayerValidateWeaponFireNoEffectServerMockCommand>());
			_networkEventManager.RegisterEvent<DestroyCubeDependency>(NetworkEvent.DestroyCubesFull, _commandFactory.Build<DestroyCubeClientCommand>());
			_networkEventManager.RegisterEvent<DestroyCubeEffectOnlyDependency>(NetworkEvent.DestroyCubeEffectOnly, _commandFactory.Build<DestroyCubeEffectOnlyClientCommand>());
			_networkEventManager.RegisterEvent<DestroyCubeNoEffectDependency>(NetworkEvent.DestroyCubeNoEffect, _commandFactory.Build<DestroyCubeNoEffectClientCommand>());
			_networkEventManagerServer.RegisterEvent<HealedCubesDependency>(NetworkEvent.HealSelf, _commandFactory.Build<SetNonWeaponHealedCubesClientCommand>());
			_networkEventManager.RegisterEvent<KillDependency>(NetworkEvent.ConfirmedKill, _commandFactory.Build<KillConfirmedClientCommand>());
			_networkEventManager.RegisterEvent<KillDependency>(NetworkEvent.ConfirmedAssist, _commandFactory.Build<AssistConfirmedClientCommand>());
			_networkEventManagerServer.RegisterEvent<AssistBonusRequestDependency>(NetworkEvent.AssistBonusRequest, _commandFactory.Build<AssistBonusServerMockCommand>());
			_networkEventManagerServer.RegisterEvent<SinglePlayerAggregateCubesCountBonusDependency>(NetworkEvent.DestroyCubesBonusRequest, _commandFactory.Build<DestroyCubesBonusServerMockCommand>());
			_networkEventManagerServer.RegisterEvent<SinglePlayerAggregateCubesCountBonusDependency>(NetworkEvent.ProtectTeamMateBonusRequest, _commandFactory.Build<ProtectTeamMateCubesBonusServerMockCommand>());
			_networkEventManagerServer.RegisterEvent<SinglePlayerAggregateCubesCountBonusDependency>(NetworkEvent.HealCubesBonusRequest, _commandFactory.Build<HealCubesBonusServerMockCommand>());
			_networkEventManagerServer.RegisterEvent<HealingAssistBonusRequestDependency>(NetworkEvent.HeallingAssistBonusRequest, _commandFactory.Build<HealingAssistBonusServerMockCommand>());
			_networkEventManagerServer.RegisterEvent<PlayerIdDependency>(NetworkEvent.AlignmentRectifierStarted, _commandFactory.Build<RemoteAlignmentRectifierClientcommand>());
			_networkEventManagerServer.RegisterEvent<HealedAllyCubesDependency>(NetworkEvent.HealAlly, _commandFactory.Build<SetAllyCubesHealedClientCommand>());
		}
	}
}
