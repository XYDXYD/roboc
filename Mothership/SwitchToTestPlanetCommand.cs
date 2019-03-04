using Authentication;
using Battle;
using Mothership.RobotConfiguration;
using Services.Simulation;
using Services.Web.Photon;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Mothership
{
	internal sealed class SwitchToTestPlanetCommand : IInjectableCommand<SwitchWorldDependency>, ICommand
	{
		private static readonly Dictionary<string, PlayerDataDependency> _expectedPlayersDict = new Dictionary<string, PlayerDataDependency>();

		private SwitchWorldDependency _dependency;

		private LoadMachineResult _firstMachineResult;

		private byte[] _colorMap;

		private GetRobotBayCustomisationsResponse _customisations;

		[Inject]
		public IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		public IAutoSaveController autoSaveController
		{
			private get;
			set;
		}

		[Inject]
		public BattleParameters battleParameters
		{
			private get;
			set;
		}

		[Inject]
		public BattlePlayersMothership battlePlayers
		{
			private get;
			set;
		}

		[Inject]
		public BattleTimer battleTimer
		{
			private get;
			set;
		}

		[Inject]
		public EnterBattleChecker battleChecker
		{
			private get;
			set;
		}

		[Inject]
		public PremiumMembership premiumMembership
		{
			private get;
			set;
		}

		[Inject]
		public WorldSwitching worldSwitching
		{
			get;
			private set;
		}

		[Inject]
		public GaragePresenter garagePresenter
		{
			private get;
			set;
		}

		[Inject]
		public RobotSanctionController robotSanctionController
		{
			private get;
			set;
		}

		[Inject]
		public RobotConfigurationDataSource robotConfigurationDataSource
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
			TaskRunner.get_Instance().Run(robotSanctionController.CheckRobotSanction(string.Empty, delegate(RobotSanctionData sanction)
			{
				TaskRunner.get_Instance().Run(OnRobotSanctionTaskSucceeded(sanction));
			}));
		}

		private void SwitchToPlanet()
		{
			Console.Log("SwitchToTestPlanetCommand: switching to test planet");
			worldSwitching.SwitchToPlanetSinglePlayer(GameModeType.TestMode, _dependency.planetToLoad);
		}

		private IEnumerator OnRobotSanctionTaskSucceeded(RobotSanctionData robotSanction)
		{
			if (robotSanction != null)
			{
				yield return garagePresenter.RefreshGarageData();
				garagePresenter.ShowGarageSlots();
				garagePresenter.LoadAndBuildRobot();
				garagePresenter.SelectCurrentGarageSlot();
			}
			else if (battleChecker.IsMachineValidForBattle())
			{
				yield return SaveThenOtherStuff();
			}
		}

		private IEnumerator SaveThenOtherStuff()
		{
			yield return autoSaveController.PerformSave();
			battlePlayers.ClearExpectedPlayersForSoloMatches();
			battleTimer.GameInitialised();
			yield return LoadMachine();
			if (_firstMachineResult == null)
			{
				yield break;
			}
			yield return LoadColours();
			if (_colorMap != null)
			{
				yield return LoadCustomisations();
				if (_customisations != null)
				{
					OnAllRequiredMachinesLoaded();
				}
			}
		}

		private IEnumerator LoadMachine()
		{
			TaskService<LoadMachineResult> loadMachineTask = serviceFactory.Create<ILoadMachineRequest>().AsTask();
			yield return new HandleTaskServiceWithError(loadMachineTask, delegate
			{
			}, delegate
			{
			}).GetEnumerator();
			if (loadMachineTask.succeeded)
			{
				_firstMachineResult = loadMachineTask.result;
			}
		}

		private IEnumerator LoadColours()
		{
			LoadMachineColorMapDependancy colourDependency = new LoadMachineColorMapDependancy(User.Username, _firstMachineResult.garageSlot);
			TaskService<byte[]> loadColourTask = serviceFactory.Create<ILoadMachineColorMapRequest, LoadMachineColorMapDependancy>(colourDependency).AsTask();
			yield return new HandleTaskServiceWithError(loadColourTask, delegate
			{
			}, delegate
			{
			}).GetEnumerator();
			if (loadColourTask.succeeded)
			{
				_colorMap = loadColourTask.result;
			}
		}

		private IEnumerator LoadCustomisations()
		{
			IGetGarageBayUniqueIdRequest getRobotUniqueIdRequest = serviceFactory.Create<IGetGarageBayUniqueIdRequest>();
			getRobotUniqueIdRequest.ClearCache();
			TaskService<UniqueSlotIdentifier> getRobotUniqueIdTask = getRobotUniqueIdRequest.AsTask();
			yield return new HandleTaskServiceWithError(getRobotUniqueIdTask, delegate
			{
			}, delegate
			{
			}).GetEnumerator();
			if (getRobotUniqueIdTask.succeeded)
			{
				string dependency = getRobotUniqueIdTask.result.ToString();
				IGetRobotBayCustomisationsRequest getCustomisationsRequest = serviceFactory.Create<IGetRobotBayCustomisationsRequest, string>(dependency);
				getCustomisationsRequest.ClearCache();
				TaskService<GetRobotBayCustomisationsResponse> getCustomisationsTask = getCustomisationsRequest.AsTask();
				yield return new HandleTaskServiceWithError(getCustomisationsTask, delegate
				{
				}, delegate
				{
				}).GetEnumerator();
				if (getCustomisationsTask.succeeded)
				{
					_customisations = getCustomisationsTask.result;
				}
			}
		}

		private void OnAllRequiredMachinesLoaded()
		{
			string currentRobotName = garagePresenter.CurrentRobotName;
			string spawnEffect = robotConfigurationDataSource.GetSpawnEffect(_customisations.SpawnEffectId);
			string deathEffect = robotConfigurationDataSource.GetDeathEffect(_customisations.DeathEffectId);
			string username = User.Username;
			string displayName = User.DisplayName;
			string robotName = currentRobotName;
			byte[] compresesdRobotData = _firstMachineResult.model.GetCompresesdRobotData();
			uint team = 0u;
			bool hasSubscription = premiumMembership.hasSubscription;
			string robotUniqueId = null;
			int cpu = 0;
			int masteryLevel = _firstMachineResult.MasteryLevel;
			AvatarInfo avatarInfo = new AvatarInfo(useCustomAvatar: false, 0);
			int[] weaponOrder = new int[0];
			PlayerDataDependency playerDataDependency = new PlayerDataDependency(username, displayName, robotName, compresesdRobotData, team, hasSubscription, robotUniqueId, cpu, masteryLevel, 0, avatarInfo, null, new AvatarInfo(useCustomAvatar: false, 0), aiPlayer: false, spawnEffect, deathEffect, weaponOrder);
			playerDataDependency.RobotColourData = _colorMap;
			playerDataDependency.WeaponOrder = new WeaponOrderSimulation(_firstMachineResult.weaponOrder.Serialise());
			battleParameters.Clear();
			_expectedPlayersDict.Clear();
			_expectedPlayersDict[playerDataDependency.PlayerName] = playerDataDependency;
			battlePlayers.SetExpectedPlayersForSoloMatches(_expectedPlayersDict);
			SwitchToPlanet();
		}
	}
}
