using Authentication;
using Fabric;
using ServerStateServiceLayer;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Login
{
	internal abstract class LoginSequenceBase : ILoginSequence
	{
		public struct MaintenanceModeState
		{
			public bool IsInMaintenanceMode;

			public string MaintenanceModeMessage;

			public MaintenanceModeState(bool IsInMaintenanceMode_, string MaintenanceModeMessage_)
			{
				IsInMaintenanceMode = IsInMaintenanceMode_;
				MaintenanceModeMessage = MaintenanceModeMessage_;
			}
		}

		protected GenericLoginController loginController;

		protected bool _isNewUser;

		protected string _abTest;

		protected string _abTestGroup;

		protected AnalyticsLoginType _analyticsLoginType;

		protected AnalyticsLaunchMode _analyticsLaunchMode = AnalyticsLaunchMode.Launcher;

		[Inject]
		internal IntroAnimationsSequenceEventObserver introAnimationsSequenceEventObserver
		{
			get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
		{
			get;
			set;
		}

		[Inject]
		internal ILoginWebservicesRequestFactory loginWebServicesRequestFactory
		{
			private get;
			set;
		}

		public abstract bool CheckFinished();

		public abstract bool CheckVideoFinished();

		public abstract void AdvanceToNextStage();

		public unsafe virtual void Initialise(GenericLoginController controller)
		{
			introAnimationsSequenceEventObserver.AddAction(new ObserverAction<IntroAnimationsSequenceEventCode>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleIntroVideoEventOccured(ref IntroAnimationsSequenceEventCode eventCode)
		{
			if (CheckVideoFinished() && eventCode == IntroAnimationsSequenceEventCode.VideoHasFinishedPlaying)
			{
				loadingIconPresenter.NotifyLoading("LoadingSplashScreenData");
				SplashLoginGUIMessage messageType = new SplashLoginGUIMessage(SplashLoginGUIMessageType.ShowTitle);
				loginController.BroadcastGUIMessage(messageType);
				AdvanceToNextStage();
			}
		}

		public abstract IEnumerator UpdateState();

		public abstract bool UserInputOccured(SplashLoginGUIMessageType inputType);

		public MaintenanceModeState CheckMaintenanceMode()
		{
			loadingIconPresenter.ChangeLoadingIconText(StringTableBase<StringTable>.Instance.GetString("strCheckingMaintenanceMode"));
			string value = string.Empty;
			if (ServEnv.Exists())
			{
				ServEnv.TryGetValue("UserMode", out value);
			}
			if (ClientConfigData.TryGetValue("MaintenanceMode", out string value2) && value2 == "true" && value != "Dev")
			{
				if (!ClientConfigData.TryGetValue("MaintenanceMessage", out string value3))
				{
					value3 = StringTableBase<StringTable>.Instance.GetString("strUnderMaintenance");
				}
				string maintenanceModeMessage_ = $"{value3}";
				return new MaintenanceModeState(IsInMaintenanceMode_: true, maintenanceModeMessage_);
			}
			return new MaintenanceModeState(IsInMaintenanceMode_: false, string.Empty);
		}

		protected void StartSharedLoginSequence()
		{
			CheckGameVersion.instance.CheckLatestBuild(loginWebServicesRequestFactory, delegate
			{
				Console.Log("Game Version Checked");
				OnClientVersionVerified();
			});
		}

		private void OnClientVersionVerified()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIMenuOpen));
			loadingIconPresenter.ChangeLoadingIconText(StringTableBase<StringTable>.Instance.GetString("strJoiningRobocraft"));
			TaskRunner.get_Instance().Run((Func<IEnumerator>)GetMaintenanceMode);
		}

		private IEnumerator GetMaintenanceMode()
		{
			TaskService<MaintenanceModeData> task = loginWebServicesRequestFactory.Create<IGetMaintenanceModeRequest>().AsTask();
			yield return task;
			if (!task.succeeded)
			{
				OnServiceError(task.behaviour);
				yield break;
			}
			MaintenanceModeData data = task.result;
			if (data.isInMaintenance)
			{
				loginController.RaiseFatalErrorDialog(StringTableBase<StringTable>.Instance.GetString("strLoginFailure"), data.serverMessage);
				yield break;
			}
			loadingIconPresenter.ChangeLoadingIconText(StringTableBase<StringTable>.Instance.GetString("strValidateUser"));
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadQualityLevelData);
			yield return ValidateUser();
		}

		private IEnumerator LoadQualityLevelData()
		{
			TaskService<QualityLevelDataAnswerData> task = loginWebServicesRequestFactory.Create<ILoadQualiltyLevelDataRequest>().AsTask();
			yield return task;
			if (task.succeeded)
			{
				new AutoQualityChooser(task.result).CalculateQualityLevels();
			}
			else
			{
				Console.LogError(task.behaviour.errorTitle + " " + task.behaviour.errorBody + " " + task.behaviour.exceptionThrown);
			}
		}

		private IEnumerator ValidateUser()
		{
			TaskService<ValidateUserRequestData> task = loginWebServicesRequestFactory.Create<IValidateUserRequest>().AsTask();
			yield return task;
			if (!task.succeeded)
			{
				OnServiceError(task.behaviour);
				yield break;
			}
			ValidateUserRequestData validateUserData = task.result;
			_isNewUser = validateUserData.isNewUser;
			_abTest = validateUserData.abTest;
			_abTestGroup = validateUserData.abTestGroup;
			Console.Log("User Validated");
			PlayerPrefs.SetInt("NewSession", 1);
			ProfileUpdateNotification.removedObsoleteCubes = validateUserData.removedObsoleteCubes;
			ProfileUpdateNotification.removedNotOwnedCubes = validateUserData.removedNotOwnedCubes;
			ProfileUpdateNotification.specialRewardTitle = validateUserData.specialRewardTitle;
			ProfileUpdateNotification.specialRewardBody = validateUserData.specialRewardBody;
			ProfileUpdateNotification.refundedObsoleteCubes = validateUserData.refundedObsoleteCubes;
			ProfileUpdateNotification.cubesHaveBeenReplaced = validateUserData.cubesHaveBeenReplaced;
			ProfileUpdateNotification.cratesRefundedTPAmount = validateUserData.cratesRefundedTPAmount;
			yield return ValidateGarages();
		}

		private IEnumerator ValidateGarages()
		{
			Console.Log("ValidateGarages login step");
			loadingIconPresenter.ChangeLoadingIconText(StringTableBase<StringTable>.Instance.GetString("strValidateGarages"));
			TaskService task = loginWebServicesRequestFactory.Create<IValidateGaragesRequest>().AsTask();
			yield return task;
			if (!task.succeeded)
			{
				OnServiceError(task.behaviour);
				yield break;
			}
			Console.Log("Garage Validated");
			int embeddedVersion = CheckGameVersion.EmbeddedVersion();
			string currentBuild = (embeddedVersion != -1) ? Convert.ToString(embeddedVersion) : "DEVBUILD";
			User.SetUserBuildNo(currentBuild);
			yield return ValidatePlayerLevel();
		}

		private IEnumerator ValidatePlayerLevel()
		{
			Console.Log("Validate Player Level login step");
			TaskService task = loginWebServicesRequestFactory.Create<IValidatePlayerLevelRequest>().AsTask();
			yield return task;
			if (!task.succeeded)
			{
				OnServiceError(task.behaviour);
				yield break;
			}
			Console.Log("Player Level Validated - Loading main level");
			FinaliseSharedLoginSequence();
		}

		protected abstract void FinaliseSharedLoginSequence();

		private void OnServiceError(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("LoadingSplashScreenData");
			Console.Log("Todo: correct handling of this exception case");
			loginController.RaiseFatalErrorDialog(behaviour.errorTitle, behaviour.errorBody);
		}

		protected IEnumerator LoadConfigData()
		{
			loadingIconPresenter.NotifyLoading("LoadingSplashScreenData", StringTableBase<StringTable>.Instance.GetString("strLoadConfigData"));
			Console.Log("Loading client data..");
			yield return ClientConfigData.Load();
			Console.Log("Client data loaded");
			loadingIconPresenter.NotifyLoadingDone("LoadingSplashScreenData");
			CheckGameVersion.instance.CheckLatestBuild();
		}
	}
}
