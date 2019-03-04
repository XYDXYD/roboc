using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mothership.GarageSkins
{
	internal class GarageBaySkinSwitcher : IInitialize, IWaitForFrameworkDestruction
	{
		private const string TOGGLE_MEGABOT_GAMEOBJECTS = "ToggleMegabotGameObjects";

		private const string TOGGLE_GAMEOBJECTS = "ToggleGameObjects";

		private const string MothershipScene = "RC_Mothership";

		private AllCustomisationsResponse _customisations;

		private bool _garageSkinActivated;

		private Scene? _activeScene;

		private Transform _activeGarageTransform;

		private bool _isMegabot;

		[Inject]
		private IServiceRequestFactory serviceFactory
		{
			get;
			set;
		}

		[Inject]
		private IGUIInputController guiInputController
		{
			get;
			set;
		}

		[Inject]
		private GarageChangedObserver garageChangedObserver
		{
			get;
			set;
		}

		[Inject]
		private GarageBaySkinNotificationObservable garageSkinNotificationObservable
		{
			get;
			set;
		}

		[Inject]
		private GarageBaySkinSelectedObserver garageSkinSelectedObserver
		{
			get;
			set;
		}

		[Inject]
		private WorldSwitching worldSwitching
		{
			get;
			set;
		}

		[Inject]
		private LoadingIconPresenter loadingIconPresenter
		{
			get;
			set;
		}

		[Inject]
		private ICPUPower cpuPower
		{
			get;
			set;
		}

		public unsafe void OnDependenciesInjected()
		{
			garageSkinSelectedObserver.AddAction(new ObserverAction<RobotBaySkinDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			garageChangedObserver.AddAction(new ObserverAction<GarageSlotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			guiInputController.OnScreenStateChange += ToggleGarageSkinGUIChanged;
			worldSwitching.OnWorldJustSwitched += ToggleGarageSkinGameObjects;
			cpuPower.RegisterOnCPULoadChanged(ToggleMegabotGameObjects);
			LoadGarageSkinsConfig();
		}

		public unsafe void OnFrameworkDestroyed()
		{
			garageChangedObserver.RemoveAction(new ObserverAction<GarageSlotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			guiInputController.OnScreenStateChange -= ToggleGarageSkinGUIChanged;
			garageSkinSelectedObserver.RemoveAction(new ObserverAction<RobotBaySkinDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			worldSwitching.OnWorldJustSwitched -= ToggleGarageSkinGameObjects;
			cpuPower.UnregisterOnCPULoadChanged(ToggleMegabotGameObjects);
		}

		private void ShowSelectedGarageSlotBaySkin(ref GarageSlotDependency garageSlot)
		{
			ShowSelectedGarageSkin(ref garageSlot.baySkinID);
		}

		private void ShowSelectedGarageSkin(ref RobotBaySkinDependency selectedBaySkin)
		{
			string selectedBaySkinId = selectedBaySkin.BaySkinID;
			ShowSelectedGarageSkin(ref selectedBaySkinId);
		}

		private void ShowSelectedGarageSkin(ref string selectedBaySkinId)
		{
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			string b = string.Empty;
			foreach (CustomisationsEntry allSkinCustomisation in _customisations.AllSkinCustomisations)
			{
				if (allSkinCustomisation.isDefault)
				{
					b = allSkinCustomisation.id;
				}
			}
			if (selectedBaySkinId != b)
			{
				TaskRunner.get_Instance().Run(LoadGarageSkin(selectedBaySkinId));
			}
			else if (_activeScene.HasValue)
			{
				UnloadGarageSkin(_activeScene.Value);
				_activeScene = null;
				_activeGarageTransform = null;
				_garageSkinActivated = false;
				garageSkinNotificationObservable.Dispatch(ref _garageSkinActivated);
			}
		}

		private void ToggleGarageSkinGUIChanged()
		{
			if (!(_activeGarageTransform == null))
			{
				bool showGarageSkinRoot = GetShowGarageSkinRoot();
				_activeGarageTransform.get_gameObject().SetActive(showGarageSkinRoot);
				if (showGarageSkinRoot)
				{
					SetSkinActive();
				}
				else
				{
					SetSkinInactive();
				}
			}
		}

		private bool GetShowGarageSkinRoot()
		{
			GuiScreens activeScreen = guiInputController.GetActiveScreen();
			return activeScreen != GuiScreens.RobotShop && activeScreen != GuiScreens.LevelRewards;
		}

		private void ToggleGarageSkinGameObjects(WorldSwitchMode mode)
		{
			if (!(_activeGarageTransform == null))
			{
				_activeGarageTransform.SendMessage("ToggleGameObjects", (object)worldSwitching.CurrentWorld);
			}
		}

		private void ToggleMegabotGameObjects(uint currentCPU)
		{
			_isMegabot = (currentCPU > cpuPower.MaxCpuPower);
			if (_activeGarageTransform != null)
			{
				_activeGarageTransform.SendMessage("ToggleMegabotGameObjects", (object)_isMegabot);
			}
		}

		private IEnumerator LoadGarageSkin(string baySkinId)
		{
			string skinSceneName = string.Empty;
			foreach (CustomisationsEntry allSkinCustomisation in _customisations.AllSkinCustomisations)
			{
				if (allSkinCustomisation.id == baySkinId)
				{
					skinSceneName = allSkinCustomisation.skinSceneName;
				}
			}
			string sceneToLoad = skinSceneName;
			if (_activeScene.HasValue)
			{
				string a = sceneToLoad;
				Scene value = _activeScene.Value;
				if (a == value.get_name())
				{
					yield break;
				}
			}
			loadingIconPresenter.NotifyLoading("LoadGarageBaySkin");
			Scene? prevScene = _activeScene;
			AsyncOperation loadop = SceneManager.LoadSceneAsync(sceneToLoad, 1);
			_activeScene = SceneManager.GetSceneByName(sceneToLoad);
			while (true)
			{
				if (loadop.get_isDone())
				{
					Scene value2 = _activeScene.Value;
					if (value2.IsValid())
					{
						break;
					}
				}
				yield return null;
			}
			if (prevScene.HasValue)
			{
				UnloadGarageSkin(prevScene.Value);
			}
			SceneManager.SetActiveScene(_activeScene.Value);
			Scene value3 = _activeScene.Value;
			GameObject[] activeSceneRootGOs = value3.GetRootGameObjects();
			_activeGarageTransform = activeSceneRootGOs[0].get_gameObject().get_transform();
			_activeGarageTransform.SendMessage("ToggleMegabotGameObjects", (object)_isMegabot);
			_activeGarageTransform.SendMessage("ToggleGameObjects", (object)worldSwitching.CurrentWorld);
			_garageSkinActivated = true;
			garageSkinNotificationObservable.Dispatch(ref _garageSkinActivated);
			loadingIconPresenter.NotifyLoadingDone("LoadGarageBaySkin");
		}

		private void SetSkinActive()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			Scene? activeScene = _activeScene;
			if (activeScene.HasValue)
			{
				SceneManager.SetActiveScene(_activeScene.Value);
			}
		}

		private void SetSkinInactive()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			SceneManager.SetActiveScene(SceneManager.GetSceneByName("RC_Mothership"));
		}

		private void UnloadGarageSkin(Scene sceneToUnload)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			SceneManager.UnloadSceneAsync(sceneToUnload);
		}

		private void LoadGarageSkinsConfig()
		{
			ILoadAllCustomisationInfoRequest loadAllCustomisationInfoRequest = serviceFactory.Create<ILoadAllCustomisationInfoRequest>();
			loadAllCustomisationInfoRequest.SetAnswer(new ServiceAnswer<AllCustomisationsResponse>(delegate(AllCustomisationsResponse response)
			{
				_customisations = response;
			}, delegate(ServiceBehaviour sb)
			{
				ErrorWindow.ShowServiceErrorWindow(sb);
			})).Execute();
		}
	}
}
