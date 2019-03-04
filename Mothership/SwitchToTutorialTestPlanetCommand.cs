using Authentication;
using Battle;
using Mothership.RobotConfiguration;
using Services.Simulation;
using Services.Web.Photon;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Mothership
{
	internal sealed class SwitchToTutorialTestPlanetCommand : IInjectableCommand<SwitchTutorialTestWorldDependency>, ICommand
	{
		private static readonly Dictionary<string, PlayerDataDependency> _expectedPlayersDict = new Dictionary<string, PlayerDataDependency>();

		private byte[] _firstMachineLoadedColors;

		private SwitchTutorialTestWorldDependency _dependency;

		private LoadMachineResult _firstMachineResult;

		private TutorialEnemyMachineData _enemyMachineData;

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
		public WorldSwitching worldSwitching
		{
			get;
			private set;
		}

		[Inject]
		public BattleTimer battleTimer
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
		public BattleParameters battleParameters
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
		public GaragePresenter garagePresenter
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

		public ICommand Inject(SwitchTutorialTestWorldDependency dependency)
		{
			_dependency = dependency;
			return this;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)SaveThenOtherStuff);
		}

		private void SwitchToPlanet()
		{
			worldSwitching.SwitchToPlanetSinglePlayer(GameModeType.TestMode, _dependency.planetToLoad, isTutorial: true);
		}

		private IEnumerator SaveThenOtherStuff()
		{
			yield return autoSaveController.PerformSaveButOnlyIfNecessary();
			battlePlayers.ClearExpectedPlayersForSoloMatches();
			yield return LoadAllRequiredMachines();
			yield return SetControlTypeToDefault();
			yield return LoadFirstMachineColors();
			yield return LoadCustomisations();
			battleTimer.GameInitialised();
			EverythingLoadedAndReady();
		}

		private IEnumerator LoadAllRequiredMachines()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			SerialTaskCollection val = new SerialTaskCollection();
			val.Add(ExecuteRequestAsTask<IGetDefaultWeaponOrderSubcategoriesRequest, List<int>>());
			val.Add(ExecuteRequestAsTask<ILoadMachineRequest, LoadMachineResult>(delegate(LoadMachineResult firstMachineResult)
			{
				_firstMachineResult = firstMachineResult;
				Console.Log("Switch to Tutorial Planet: First machine loaded");
			}));
			val.Add(ExecuteRequestAsTask<ILoadTutorialEnemyMachineRequest, TutorialEnemyMachineData>(delegate(TutorialEnemyMachineData enemyMachineData)
			{
				_enemyMachineData = enemyMachineData;
				Console.Log("Switch to Tutorial Planet: Enemy machine loaded");
			}));
			return (IEnumerator)val;
		}

		private IEnumerator ExecuteRequestAsTask<TRequestType, TReturnData>(Action<TReturnData> onComplete = null) where TRequestType : class, IServiceRequest, IAnswerOnComplete<TReturnData>
		{
			TaskService<TReturnData> taskService = serviceFactory.Create<TRequestType>().AsTask();
			yield return new HandleTaskServiceWithError(taskService, delegate
			{
			}, delegate
			{
			}).GetEnumerator();
			if (taskService.succeeded)
			{
				onComplete?.Invoke(taskService.result);
			}
		}

		private IEnumerator SetControlTypeToDefault()
		{
			_firstMachineResult.controlSettings.controlType = ControlType.CameraControl;
			GarageSlotControlsDependency dependency = new GarageSlotControlsDependency(_firstMachineResult.garageSlot, _firstMachineResult.controlSettings);
			TaskService<ControlSettings> taskService = serviceFactory.Create<ISetGarageSlotControlsRequest, GarageSlotControlsDependency>(dependency).AsTask();
			yield return new HandleTaskServiceWithError(taskService, delegate
			{
			}, delegate
			{
			}).GetEnumerator();
			Console.Log((!taskService.succeeded) ? "Save Controls Failed" : "Slot Controls Saved");
		}

		private IEnumerator LoadFirstMachineColors()
		{
			_firstMachineLoadedColors = new byte[0];
			LoadMachineColorMapDependancy dependency = new LoadMachineColorMapDependancy(User.Username, _firstMachineResult.garageSlot);
			TaskService<byte[]> taskService = serviceFactory.Create<ILoadMachineColorMapRequest, LoadMachineColorMapDependancy>(dependency).AsTask();
			yield return new HandleTaskServiceWithError(taskService, delegate
			{
			}, delegate
			{
			}).GetEnumerator();
			if (taskService.succeeded)
			{
				_firstMachineLoadedColors = taskService.result;
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

		private void EverythingLoadedAndReady()
		{
			byte[] firstMachineLoadedColors = _firstMachineLoadedColors;
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
			int masteryLevel = 1;
			int tier = 0;
			AvatarInfo avatarInfo = new AvatarInfo(useCustomAvatar: false, 0);
			string clanName = null;
			AvatarInfo clanAvatarInfo = new AvatarInfo(useCustomAvatar: false, 0);
			bool aiPlayer = false;
			int[] weaponOrder = new int[0];
			PlayerDataDependency playerDataDependency = new PlayerDataDependency(username, displayName, robotName, compresesdRobotData, team, hasSubscription, robotUniqueId, cpu, masteryLevel, tier, avatarInfo, clanName, clanAvatarInfo, aiPlayer, spawnEffect, deathEffect, weaponOrder);
			playerDataDependency.RobotColourData = firstMachineLoadedColors;
			playerDataDependency.WeaponOrder = new WeaponOrderSimulation(_firstMachineResult.weaponOrder.Serialise());
			clanName = currentRobotName;
			robotUniqueId = currentRobotName;
			robotName = currentRobotName;
			compresesdRobotData = _enemyMachineData.model.GetCompresesdRobotData();
			team = 1u;
			aiPlayer = false;
			displayName = "0";
			tier = 0;
			masteryLevel = 1;
			cpu = 0;
			clanAvatarInfo = new AvatarInfo(useCustomAvatar: false, 0);
			username = null;
			avatarInfo = new AvatarInfo(useCustomAvatar: false, 0);
			hasSubscription = true;
			weaponOrder = new int[0];
			PlayerDataDependency playerDataDependency2 = new PlayerDataDependency(clanName, robotUniqueId, robotName, compresesdRobotData, team, aiPlayer, displayName, tier, masteryLevel, cpu, clanAvatarInfo, username, avatarInfo, hasSubscription, "Spawn", "Explosion", weaponOrder);
			playerDataDependency2.RobotColourData = _enemyMachineData.colourData;
			battleParameters.Clear();
			_expectedPlayersDict.Clear();
			_expectedPlayersDict[playerDataDependency.PlayerName] = playerDataDependency;
			_expectedPlayersDict[playerDataDependency2.PlayerName] = playerDataDependency2;
			battlePlayers.SetExpectedPlayersForSoloMatches(_expectedPlayersDict);
			SwitchToPlanet();
		}
	}
}
