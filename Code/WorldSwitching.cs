using Fabric;
using Mothership;
using Svelto.Command;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

internal sealed class WorldSwitching : IDispatchWorldSwitching
{
	private sealed class SwitchWorldStaticMemory
	{
		public Vector3 avatarPosition;

		public Vector3 avatarRotation;

		public GameModeType gameModeType;

		public bool IsRanked;

		public bool IsBrawl;

		public bool IsCustomGame;

		public bool isStarted;

		public bool isTutorial;

		public WorldSwitchMode switchingFromWorld = WorldSwitchMode.GarageMode;

		public WorldSwitchMode switchingToWorld = WorldSwitchMode.GarageMode;

		public WorldSwitchMode currentWorld = WorldSwitchMode.GarageMode;

		public float currentWorldStartTime;

		public string additionalLoadingScreenMessage;

		public string CampaignID;

		public string CampaignName;

		public int CampaignDifficulty;
	}

	private const float DUMMY_SWITCH_TIME = 0.5f;

	private string[] PRACTICE_MAPS = new string[6]
	{
		"RC_Planet_Earth_01_BA",
		"RC_Planet_Earth_02_BA",
		"RC_Planet_Mars_02_BA",
		"RC_Planet_Mars_03_BA",
		"RC_Planet_Neptune_02_BA",
		"RC_Planet_Neptune_03_BA"
	};

	private static SwitchWorldStaticMemory _cache = new SwitchWorldStaticMemory();

	public float worldSwitchTime;

	private bool _isLoading;

	private ParallelTaskCollection _OnWorldIsSwitching = new ParallelTaskCollection();

	public float Progress
	{
		get;
		private set;
	}

	public bool IsComplete
	{
		get;
		private set;
	}

	[Inject]
	public InputController inputController
	{
		private get;
		set;
	}

	[Inject]
	public ICommandFactory commandFactory
	{
		private get;
		set;
	}

	[Inject]
	public IGameObjectFactory gofactory
	{
		private get;
		set;
	}

	[Inject]
	public GaragePresenter garage
	{
		private get;
		set;
	}

	[Inject]
	public IGUIInputControllerMothership guiInputController
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
	public SwitchingToTestModeObservable testModeObservable
	{
		private get;
		set;
	}

	public ParallelTaskCollection OnWorldIsSwitching => _OnWorldIsSwitching;

	public WorldSwitchMode CurrentWorld => _cache.currentWorld;

	public WorldSwitchMode SwitchingTo => _cache.switchingToWorld;

	public WorldSwitchMode SwitchingFrom => _cache.switchingFromWorld;

	public float CurrentWorldStartTime => _cache.currentWorldStartTime;

	public static string AdditionalLoadingScreenMessage => _cache.additionalLoadingScreenMessage;

	public bool SwitchingToSimulation => _cache.switchingToWorld == WorldSwitchMode.SimulationMP || _cache.switchingToWorld == WorldSwitchMode.SimulationSP;

	public bool SwitchingFromSimulation => _cache.switchingFromWorld == WorldSwitchMode.SimulationMP || _cache.switchingFromWorld == WorldSwitchMode.SimulationSP;

	public event Action<WorldSwitchMode> OnWorldJustSwitched;

	public static GameModeType GetGameModeType()
	{
		return _cache.gameModeType;
	}

	public static bool IsMultiplayer()
	{
		return _cache.switchingToWorld == WorldSwitchMode.SimulationMP;
	}

	public static bool IsRanked()
	{
		return _cache.IsRanked;
	}

	public static bool IsBrawl()
	{
		return _cache.IsBrawl;
	}

	public static bool IsCustomGame()
	{
		return _cache.IsCustomGame;
	}

	public static bool IsTutorial()
	{
		return _cache.isTutorial;
	}

	public static bool IsInBuildMode()
	{
		return _cache.currentWorld == WorldSwitchMode.BuildMode;
	}

	public static bool IsInGarageMode()
	{
		return _cache.currentWorld == WorldSwitchMode.GarageMode;
	}

	public static string GetCampaignID()
	{
		return _cache.CampaignID;
	}

	public static string GetCampaignName()
	{
		return _cache.CampaignName;
	}

	public static int GetCampaignDifficulty()
	{
		return _cache.CampaignDifficulty;
	}

	public string GetPracticeModePlanet()
	{
		string text = PRACTICE_MAPS[Random.Range(0, PRACTICE_MAPS.Length)];
		Console.Log("Loading Practice Map: " + text);
		return text;
	}

	public void StartMothershipWithLastUsedMode()
	{
		if (_cache.isStarted)
		{
			RestoreAvatarPosition();
		}
		else
		{
			_cache.isStarted = true;
		}
		_cache.currentWorld = _cache.switchingToWorld;
		_cache.currentWorldStartTime = Time.get_time();
		SafeEvent.SafeRaise<WorldSwitchMode>(this.OnWorldJustSwitched, _cache.currentWorld);
	}

	public void RestoreAvatarPosition()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		CharacterMotorEx characterMotorEx = Object.FindObjectOfType<CharacterMotorEx>();
		characterMotorEx.SetPreviousPositionOrientation(_cache.avatarPosition, _cache.avatarRotation);
		Console.Log("Returning avatar to the position of:" + _cache.avatarPosition.x + "," + _cache.avatarPosition.y + "," + _cache.avatarPosition.z);
	}

	public void StartPlanet()
	{
		_cache.currentWorld = _cache.switchingToWorld;
		_cache.currentWorldStartTime = Time.get_time();
		SafeEvent.SafeRaise<WorldSwitchMode>(this.OnWorldJustSwitched, _cache.currentWorld);
	}

	public void StartBuildMode()
	{
		_cache.currentWorld = WorldSwitchMode.BuildMode;
		_cache.currentWorldStartTime = Time.get_time();
		guiInputController.ShowScreen(GuiScreens.BuildMode);
		SafeEvent.SafeRaise<WorldSwitchMode>(this.OnWorldJustSwitched, _cache.currentWorld);
	}

	private void StartGarage()
	{
		_cache.currentWorld = WorldSwitchMode.GarageMode;
		_cache.currentWorldStartTime = Time.get_time();
		guiInputController.ShowScreen(GuiScreens.Garage);
		SafeEvent.SafeRaise<WorldSwitchMode>(this.OnWorldJustSwitched, _cache.currentWorld);
	}

	public void SwitchToLastMothershipGameMode(bool fastSwitch)
	{
		_cache.switchingToWorld = GetLastMothershipGameMode();
		_cache.switchingFromWorld = _cache.currentWorld;
		TaskRunner.get_Instance().Run(SwitchToMothershipInternal(fastSwitch));
	}

	private static WorldSwitchMode GetLastMothershipGameMode()
	{
		return _cache.switchingFromWorld;
	}

	public void SwitchToMothershipStartTutorialReloadContext()
	{
		_cache.isTutorial = true;
		SwitchToLastMothershipGameMode(fastSwitch: true);
	}

	public void SwitchToMothershipFromTutorialSimulation()
	{
		_cache.switchingToWorld = WorldSwitchMode.GarageMode;
		_cache.switchingFromWorld = _cache.currentWorld;
		_cache.isTutorial = false;
		TaskRunner.get_Instance().Run(SwitchToMothershipInternal(fastSwitch: true));
	}

	public void SwitchToPlanetMultiplayer(SwitchWorldDependency dependency)
	{
		_cache.switchingFromWorld = _cache.currentWorld;
		SetupMultiplayer();
		SetRanked(dependency.IsRanked);
		SetBrawl(dependency.IsBrawl);
		SetCustomGame(dependency.IsCustomGame);
		SetGameModeType(dependency.gameModeType);
		TaskRunner.get_Instance().Run(SwitchToPlanet(dependency.planetToLoad));
	}

	public void SwitchToPlanetSinglePlayer(GameModeType gameMode, string planetToLoad, bool isTutorial = false)
	{
		_cache.switchingFromWorld = _cache.currentWorld;
		_cache.isTutorial = isTutorial;
		SetGameModeType(gameMode);
		SetupSinglePlayer();
		if (gameMode == GameModeType.TestMode)
		{
			TaskRunner.get_Instance().Run(SwitchToTest(planetToLoad));
		}
		else
		{
			TaskRunner.get_Instance().Run(SwitchToPlanet(planetToLoad));
		}
	}

	public void SwitchToPlanetSinglePlayerCampaign(SwitchSinglePlayerCampaignWorldDependency dependency)
	{
		_cache.switchingFromWorld = _cache.currentWorld;
		_cache.isTutorial = false;
		_cache.CampaignID = dependency.campaignID;
		_cache.CampaignDifficulty = dependency.campaignDifficulty;
		_cache.CampaignName = dependency.campaignName;
		SetGameModeType(dependency.gameModeType);
		SetupSinglePlayer();
		TaskRunner.get_Instance().Run(SwitchToPlanet(dependency.planetToLoad));
	}

	public void SetAvatarPositionAndRotation(Vector3 position, Vector3 orientation)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		CharacterMotorEx characterMotorEx = Object.FindObjectOfType(typeof(CharacterMotorEx)) as CharacterMotorEx;
		Transform transform = characterMotorEx.get_transform();
		SwitchWorldStaticMemory cache = _cache;
		Vector3 position2 = transform.get_position();
		float x = position2.x;
		Vector3 position3 = transform.get_position();
		float y = position3.y;
		Vector3 position4 = transform.get_position();
		cache.avatarPosition = new Vector3(x, y, position4.z);
		_cache.avatarRotation = characterMotorEx.LookEulerian;
		characterMotorEx.SetPreviousPositionOrientation(position, orientation);
	}

	public void SwitchToBuildModeFromGarage(SwitchWorldDependency dependency)
	{
		_cache.switchingFromWorld = WorldSwitchMode.GarageMode;
		_cache.switchingToWorld = WorldSwitchMode.BuildMode;
		TaskRunner.get_Instance().Run(SwitchToBuild(dependency.planetToLoad, dependency.fastSwitch));
	}

	public void SwitchToGarageFromBuildMode(SwitchWorldDependency dependency)
	{
		_cache.switchingFromWorld = WorldSwitchMode.BuildMode;
		_cache.switchingToWorld = WorldSwitchMode.GarageMode;
		Console.Log("Triggering auto-save on leaving build mode and returning to garage");
		TaskRunner.get_Instance().Run(autoSaveController.PerformSaveButOnlyIfNecessary());
		TaskRunner.get_Instance().Run(SwitchToGarage(dependency.planetToLoad, dependency.fastSwitch, dependency.ContinueWith));
	}

	public void SetAdditionaLoadingScreenMessage(string message)
	{
		_cache.additionalLoadingScreenMessage = message;
	}

	private void EnableLoadingScreen(string sceneName)
	{
		GameObject val = gofactory.Build("Loading_" + sceneName);
		if (AdditionalLoadingScreenMessage != null)
		{
			GenericLoadingScreen component = val.GetComponent<GenericLoadingScreen>();
			component.additionalMessageLabel.set_text(AdditionalLoadingScreenMessage);
			component.additionalMessageContainer.get_gameObject().SetActive(true);
		}
		val.SetActive(true);
	}

	private void EnableLoadingScreenMultiplayer(string sceneName, GameModeType gameModeType)
	{
		GameObject val = gofactory.Build("BattleLoadingUI");
		GameObject val2 = gofactory.Build("ImprovedBattleLoading");
		val2.get_transform().set_parent(val.get_transform());
		val.get_transform().set_parent(null);
		Object.DontDestroyOnLoad(val);
		val2.SetActive(true);
		MultiplayerLoadingScreen componentInChildren = val2.GetComponentInChildren<MultiplayerLoadingScreen>();
		componentInChildren.SetSceneName(sceneName);
		componentInChildren.SetGameModeHints(gameModeType);
		componentInChildren.CustomHidePanels(gameModeType);
	}

	private void ExtraStuffToDoWhenLeavingTheMothership()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
		CharacterMotorEx characterMotorEx = Object.FindObjectOfType(typeof(CharacterMotorEx)) as CharacterMotorEx;
		Transform transform = characterMotorEx.get_transform();
		SwitchWorldStaticMemory cache = _cache;
		Vector3 position = transform.get_position();
		float x = position.x;
		Vector3 position2 = transform.get_position();
		float y = position2.y;
		Vector3 position3 = transform.get_position();
		cache.avatarPosition = new Vector3(x, y, position3.z);
		_cache.avatarRotation = characterMotorEx.LookEulerian;
	}

	private IEnumerator LoadLevelAsync(string name)
	{
		if (!_isLoading)
		{
			DateTime then = DateTime.UtcNow;
			_isLoading = true;
			AsyncOperation loadTask = SceneManager.LoadSceneAsync(name);
			while (!loadTask.get_isDone())
			{
				Progress = loadTask.get_progress();
				yield return null;
			}
			IsComplete = true;
			TimeSpan lapsed = DateTime.UtcNow - then;
			Console.Log("Level " + name + " loaded in " + lapsed.TotalSeconds + " seconds");
			yield break;
		}
		throw new Exception("MASSIVE ERROR INCOMING: Level loaded twice!");
	}

	private void SetGameModeType(GameModeType gameModeType)
	{
		Console.Log("Set desired game mode to: " + gameModeType.ToString());
		_cache.gameModeType = gameModeType;
	}

	private void SetRanked(bool ranked)
	{
		_cache.IsRanked = ranked;
	}

	private void SetBrawl(bool brawl)
	{
		_cache.IsBrawl = brawl;
	}

	private void SetCustomGame(bool customGame)
	{
		_cache.IsCustomGame = customGame;
	}

	private void SetupMultiplayer()
	{
		_cache.switchingToWorld = WorldSwitchMode.SimulationMP;
	}

	private void SetupSinglePlayer()
	{
		_cache.switchingToWorld = WorldSwitchMode.SimulationSP;
		SetRanked(ranked: false);
		SetBrawl(brawl: false);
		SetCustomGame(customGame: false);
	}

	private IEnumerator SwitchToMothershipInternal(bool fastSwitch)
	{
		FabricManager.get_Instance().Stop(0f);
		inputController.Enabled = false;
		Console.Log("Switch to Mothership. Fast == " + fastSwitch);
		if (!fastSwitch)
		{
			yield return (object)new WaitForSecondsEnumerator(worldSwitchTime);
		}
		string loadingScreenName = (_cache.switchingToWorld != 0) ? "RC_Mothership" : "RC_BuildMode";
		EnableLoadingScreen(loadingScreenName);
		yield return OnWorldIsSwitching;
		TaskRunner.StopAndCleanupAllDefaultSchedulers();
		TaskRunner.get_Instance().Run(LoadLevelAsync("RC_Mothership"));
	}

	private IEnumerator SwitchToPlanet(string planetLevelName)
	{
		FabricManager.get_Instance().Stop(0f);
		Console.Log("Switch to " + planetLevelName);
		inputController.Enabled = false;
		ExtraStuffToDoWhenLeavingTheMothership();
		EnableLoadingScreenMultiplayer(planetLevelName, GetGameModeType());
		yield return OnWorldIsSwitching;
		TaskRunner.StopAndCleanupAllDefaultSchedulers();
		TaskRunner.get_Instance().Run(LoadLevelAsync(planetLevelName));
	}

	private IEnumerator SwitchToTest(string planetLevelName)
	{
		Console.Log("Switch to " + planetLevelName);
		testModeObservable.Dispatch();
		inputController.Enabled = false;
		ExtraStuffToDoWhenLeavingTheMothership();
		EnableLoadingScreen(planetLevelName);
		yield return OnWorldIsSwitching;
		TaskRunner.StopAndCleanupAllDefaultSchedulers();
		TaskRunner.get_Instance().Run(LoadLevelAsync(planetLevelName));
	}

	private IEnumerator SwitchToBuild(string planetLevelName, bool fastSwitch)
	{
		FabricManager.get_Instance().Stop(0f);
		inputController.Enabled = false;
		EnableLoadingScreen(planetLevelName);
		yield return OnWorldIsSwitching;
		if (!fastSwitch)
		{
			yield return LoadBuildMode();
		}
		inputController.Enabled = true;
		StartBuildMode();
	}

	private IEnumerator LoadBuildMode()
	{
		DateTime then = DateTime.UtcNow;
		if (garage.isBusyBuilding)
		{
			while (garage.isBusyBuilding)
			{
				yield return null;
			}
		}
		else
		{
			yield return BuildMachine();
		}
		double elapsedSeconds = (DateTime.UtcNow - then).TotalSeconds;
		if (elapsedSeconds < 0.5)
		{
			yield return (object)new WaitForSecondsEnumerator((float)(0.5 - elapsedSeconds));
		}
	}

	private IEnumerator BuildMachine()
	{
		bool building = true;
		garage.LoadAndBuildRobotInMothership(delegate
		{
			building = false;
		});
		while (building)
		{
			yield return null;
		}
	}

	private IEnumerator SwitchToGarage(string planetLevelName, bool fastSwitch, Action continueWith)
	{
		FabricManager.get_Instance().Stop(0f);
		inputController.Enabled = false;
		EnableLoadingScreen(planetLevelName);
		yield return OnWorldIsSwitching;
		if (!fastSwitch)
		{
			float timer = 0.5f;
			while (true)
			{
				float num;
				timer = (num = timer - Time.get_deltaTime());
				if (!(num >= 0f))
				{
					break;
				}
				yield return null;
			}
		}
		inputController.Enabled = true;
		StartGarage();
		continueWith?.Invoke();
	}
}
