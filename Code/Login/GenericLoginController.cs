using ChatServiceLayer.Photon;
using Fabric;
using LobbyServiceLayer.Photon;
using Services.Web.Photon;
using SinglePlayerServiceLayer.Photon;
using SocialServiceLayer.Photon;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Utility;

namespace Login
{
	internal class GenericLoginController : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		private class ErrorState
		{
			public string errorMessage;

			public string stackTrace;

			public bool errorIsFatal;

			public ErrorState(string errorMessage_, string stackTrack_, bool isFatal_)
			{
				errorMessage = errorMessage_;
				stackTrace = stackTrack_;
				errorIsFatal = isFatal_;
			}
		}

		private DateTime _typeTime;

		private ErrorState _loginErrorState;

		private ITaskRoutine _taskRoutine;

		private ITaskRoutine _loadingRoutine;

		private ITaskRoutine _checkUsernameRoutine;

		private GenericLoginView _view;

		private ISplashLoginDialogController _currentSplashLoginDialog;

		[Inject]
		internal ILoginSequence loginSequence
		{
			private get;
			set;
		}

		[Inject]
		internal SplashLoginDialogFactory splashLoginDialogFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IPlatformUtilities platformUtilities
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		public void SetView(GenericLoginView view)
		{
			_view = view;
		}

		private void OnGenericError(string condition, string stackTrace, LogType type, Thread thread)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Invalid comparison between Unknown and I4
			if ((int)type == 4 || (int)type == 1)
			{
				if (condition.Contains($"Reason: {((short)125).ToString()}"))
				{
					RaiseInternallErrorDialog(new ErrorState(StringTableBase<StringTable>.Instance.GetString("strUnderMaintenance"), stackTrace, isFatal_: true));
				}
				else if (condition.Contains(1717.ToString()))
				{
					RaiseFatalErrorDialog(StringTableBase<StringTable>.Instance.GetString("strLoginFailure"), condition);
				}
				else
				{
					RaiseInternallErrorDialog(new ErrorState(condition, stackTrace, isFatal_: false));
				}
			}
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			splashLoginDialogFactory.SetParentObject(_view.get_gameObject());
			UnityLoggerCallback.AddLogger(OnGenericError);
			PhotonWebServicesUtility.Instance.AddCCUExceededEventHandler(OnCCuExceeded);
			PhotonWebServicesUtility.Instance.AddCCUPassedEventHandler(OnCCUPassed);
			loginSequence.Initialise(this);
			TaskRunner.get_Instance().Run((Func<IEnumerator>)ClearAnyErrorAndRestartLoginSequence);
			_view.SetVersion();
		}

		private void OnCCuExceeded(int queuePosition, List<string> textStrings)
		{
			RaiseNewDialog(typeof(SplashLoginTooHighCCUController));
			SplashLoginTooHighCCUController splashLoginTooHighCCUController = (SplashLoginTooHighCCUController)GetCurrentDialog();
			if (splashLoginTooHighCCUController != null)
			{
				string str;
				if (textStrings != null && textStrings.Count == 3)
				{
					splashLoginTooHighCCUController.SetHeaderAndBody(textStrings[0], textStrings[1]);
					str = textStrings[2];
				}
				else
				{
					str = StringTableBase<StringTable>.Instance.GetString("strQueueMessageText");
					splashLoginTooHighCCUController.SetHeaderAndBody(StringTableBase<StringTable>.Instance.GetString("strDisplayMessageTitle"), StringTableBase<StringTable>.Instance.GetString("strDisplayMessageBody"));
				}
				splashLoginTooHighCCUController.SetQueuePositionText(str + " " + queuePosition.ToString());
			}
		}

		private void OnCCUPassed()
		{
			ISplashLoginDialogController currentDialog = GetCurrentDialog();
			if (currentDialog != null)
			{
				CloseCurrentDialog();
			}
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			PhotonWebServicesUtility.Instance.RemoveCCUExceededEventHandler(OnCCuExceeded);
			PhotonWebServicesUtility.Instance.RemoveCCUPassedEventHandler(OnCCUPassed);
			TaskRunner.StopAndCleanupAllDefaultSchedulers();
			Console.Log("GENERIC LOGIN CONTROLLER FRAMEWORK DESTROYED");
			UnityLoggerCallback.RemoveLogger(OnGenericError);
			ClearUpServiceLayer();
			GenericErrorCatch.Init();
		}

		private static void ClearUpServiceLayer()
		{
			PhotonWebServicesUtility.TearDown();
			PhotonChatUtility.TearDown();
			PhotonSocialUtility.TearDown();
			PhotonSinglePlayerUtility.TearDown();
			PhotonLobbyUtility.TearDown();
		}

		private IEnumerator AdvanceLoginSequence()
		{
			while (true)
			{
				if (_loginErrorState == null)
				{
					yield return loginSequence.UpdateState();
					if (loginSequence.CheckFinished())
					{
						break;
					}
				}
				if (_loginErrorState != null)
				{
					yield break;
				}
			}
			_loadingRoutine = InitialiseAndLoadLevel.StartLoadGame(justRegisteredUser: false, loadingIconPresenter.loadingScreen);
		}

		public void HandleMessage(object message)
		{
			SplashLoginGUIMessage splashLoginGUIMessage = message as SplashLoginGUIMessage;
			if (splashLoginGUIMessage == null)
			{
				return;
			}
			if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.UsernameSubmitted && _checkUsernameRoutine != null)
			{
				_checkUsernameRoutine.Stop();
				_checkUsernameRoutine = null;
			}
			if (loginSequence.UserInputOccured(splashLoginGUIMessage.Message))
			{
				return;
			}
			if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.EnterUsernameTextEntryFieldChanged)
			{
				if (_currentSplashLoginDialog != null && _currentSplashLoginDialog.GetType() == typeof(EnterUsernameDialogController))
				{
					_typeTime = DateTime.UtcNow;
					if (_checkUsernameRoutine == null)
					{
						_checkUsernameRoutine = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)IsIdentifierAvailable);
					}
					_checkUsernameRoutine.Start((Action<PausableTaskException>)null, (Action)null);
				}
			}
			else if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.ShowTitle)
			{
				_view.ShowTitle(show: true);
			}
		}

		private IEnumerator IsIdentifierAvailable()
		{
			while (true)
			{
				if ((DateTime.UtcNow - _typeTime).TotalSeconds < 2.0)
				{
					yield return null;
					continue;
				}
				string currentIdentifier = (string)GetDialogEntry(SplashLoginEntryField.Field_Identifier);
				Console.Log("IsIdentifierAvailable: " + currentIdentifier);
				IEnumerator enumerator = platformUtilities.CheckIdentifierAvailableDuringEntry(currentIdentifier);
				yield return enumerator;
				switch ((UsernameValidDuringEntryStatus)enumerator.Current)
				{
				case UsernameValidDuringEntryStatus.EntryIsValid:
					BroadcastGUIMessage(new SplashLoginGUIMessage(SplashLoginGUIMessageType.ChangedTextEntryIsValid));
					break;
				case UsernameValidDuringEntryStatus.EntryTooLong:
					BroadcastGUIMessage(new SplashLoginGUIMessage(SplashLoginGUIMessageType.ChangedTextEntryIsTooLong));
					break;
				case UsernameValidDuringEntryStatus.EntryTooShort:
					BroadcastGUIMessage(new SplashLoginGUIMessage(SplashLoginGUIMessageType.ChangedTextEntryIsTooShort));
					break;
				case UsernameValidDuringEntryStatus.EntryContainsProfanity:
					BroadcastGUIMessage(new SplashLoginGUIMessage(SplashLoginGUIMessageType.ChangedTextEntryContainsProfanity));
					break;
				case UsernameValidDuringEntryStatus.NameAlreadyUsed:
					BroadcastGUIMessage(new SplashLoginGUIMessage(SplashLoginGUIMessageType.ChangedTextEntryNameInUse));
					break;
				case UsernameValidDuringEntryStatus.EntryIsInvalidSomeOtherReasons:
					BroadcastGUIMessage(new SplashLoginGUIMessage(SplashLoginGUIMessageType.ChangedTextEntrySomeOtherError));
					break;
				}
				if (currentIdentifier == string.Empty || currentIdentifier == (string)GetDialogEntry(SplashLoginEntryField.Field_Identifier))
				{
					break;
				}
			}
			_checkUsernameRoutine.Stop();
		}

		public object GetDialogEntry(SplashLoginEntryField field)
		{
			if (_currentSplashLoginDialog != null)
			{
				return _currentSplashLoginDialog.GetEntry(field);
			}
			return null;
		}

		public void RaiseNewDialog(Type dialogControllerType)
		{
			if (_currentSplashLoginDialog != null)
			{
				CloseCurrentDialog();
			}
			_currentSplashLoginDialog = splashLoginDialogFactory.CreateDialog(dialogControllerType);
		}

		public void RaiseFatalErrorDialog(string titleMessage, string bodyMessage, string stackTrace = null)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIMenuClosed));
			RemoteLogError("Fatal Login Flow Error:", titleMessage, stackTrace);
			KillActiveLoadingtasks();
			SplashLoginErrorDialogConfiguration configuration = new SplashLoginErrorDialogConfiguration(titleMessage, bodyMessage, SplashLoginErrorDialogConfiguration.ButtonType.Quit);
			_loginErrorState = new ErrorState(titleMessage, string.Empty, isFatal_: true);
			ISplashLoginDialogController splashLoginDialogController = splashLoginDialogFactory.CreateDialog(typeof(SplashLoginErrorDialogController));
			(splashLoginDialogController as SplashLoginErrorDialogController).SetConfiguration(configuration);
		}

		public void RaiseInformationDialog(string titleMessage, string bodyMessage)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIMenuClosed));
			SplashLoginErrorDialogConfiguration configuration = new SplashLoginErrorDialogConfiguration(titleMessage, bodyMessage, SplashLoginErrorDialogConfiguration.ButtonType.OK);
			ISplashLoginDialogController splashLoginDialogController = splashLoginDialogFactory.CreateDialog(typeof(SplashLoginErrorDialogController));
			(splashLoginDialogController as SplashLoginErrorDialogController).SetConfiguration(configuration);
		}

		private void RaiseInternallErrorDialog(ErrorState errorState)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIMenuClosed));
			_loginErrorState = errorState;
			RemoteLogError("Login Flow Error occured:", errorState.errorMessage, errorState.stackTrace);
			SplashLoginErrorDialogConfiguration configuration = new SplashLoginErrorDialogConfiguration(StringTableBase<StringTable>.Instance.GetString("strLoginFailure"), errorState.errorMessage, SplashLoginErrorDialogConfiguration.ButtonType.OK, delegate
			{
				TaskRunner.get_Instance().Run(ClearAnyErrorAndRestartLoginSequence());
			});
			ISplashLoginDialogController splashLoginDialogController = splashLoginDialogFactory.CreateDialog(typeof(SplashLoginErrorDialogController));
			(splashLoginDialogController as SplashLoginErrorDialogController).SetConfiguration(configuration);
		}

		private IEnumerator ClearAnyErrorAndRestartLoginSequence()
		{
			yield return null;
			_loginErrorState = null;
			_taskRoutine = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumerator(AdvanceLoginSequence());
			_taskRoutine.Start((Action<PausableTaskException>)OnMainFail, (Action)null);
		}

		private void OnMainFail(PausableTaskException exception)
		{
			LocalLogError("exception occured in main task:" + ((Exception)exception).Message);
			if (((Exception)exception).InnerException != null)
			{
				RaiseInternallErrorDialog(new ErrorState(((Exception)exception).InnerException.Message, ((Exception)exception).StackTrace, isFatal_: false));
			}
			else
			{
				RaiseInternallErrorDialog(new ErrorState(((Exception)exception).Message, ((Exception)exception).StackTrace, isFatal_: false));
			}
		}

		private void KillActiveLoadingtasks()
		{
			if (_loadingRoutine != null)
			{
				_loadingRoutine.Stop();
			}
			TaskRunner.StopAndCleanupAllDefaultSchedulers();
		}

		private void LocalLogError(string errorMessage)
		{
			Console.LogError(errorMessage);
		}

		private void RemoteLogError(string title, string message, string stackTrace = null)
		{
			RemoteLogger.Error(title, message, stackTrace);
		}

		public void CloseCurrentDialog()
		{
			if (_currentSplashLoginDialog != null)
			{
				_currentSplashLoginDialog.Close();
				_currentSplashLoginDialog = null;
			}
		}

		public ISplashLoginDialogController GetCurrentDialog()
		{
			return _currentSplashLoginDialog;
		}

		public void BroadcastGUIMessage(SplashLoginGUIMessage messageType)
		{
			_view.BroadcastMessage(messageType);
		}

		public void Initialise()
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			Console.Log("Date: " + DateTime.Now);
			Console.Log("UTC Date: " + DateTime.UtcNow);
			Console.Log("Memory Size: " + SystemInfo.get_systemMemorySize());
			Console.Log("Processor Type: " + SystemInfo.get_processorType());
			Console.Log("OS: " + SystemInfo.get_operatingSystem());
			Console.Log("Unity Version: " + Application.get_unityVersion());
			Console.Log("Platform: " + Application.get_platform());
			Console.Log("Shader Level: " + SystemInfo.get_graphicsShaderLevel());
			Console.Log("GPU: " + SystemInfo.get_graphicsDeviceName());
			Console.Log("Renderer: " + SystemInfo.get_graphicsDeviceType());
			Console.Log("GPU ID: " + SystemInfo.get_graphicsDeviceID());
			UnityLoggerCallback.Init();
			MainThreadCaller.Init();
		}

		public static void ValidateScreenResolution()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			Application.set_runInBackground(true);
			Application.set_targetFrameRate(60);
			if (!PlayerPrefs.HasKey("FirstTimeScreenResolutionSet"))
			{
				Resolution currentResolution = Screen.get_currentResolution();
				Screen.SetResolution(currentResolution.get_width(), currentResolution.get_height(), true);
				PlayerPrefs.SetInt("FirstTimeScreenResolutionSet", 1);
			}
			Resolution[] resolutions = Screen.get_resolutions();
			bool flag = false;
			for (int i = 0; i < resolutions.Length; i++)
			{
				Resolution val = resolutions[i];
				Resolution currentResolution2 = Screen.get_currentResolution();
				if (currentResolution2.get_width() == val.get_width() && currentResolution2.get_height() == val.get_height() && currentResolution2.get_width() >= 1024 && currentResolution2.get_height() >= 720 && currentResolution2.get_width() <= 3840 && currentResolution2.get_height() <= 2160)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Resolution currentResolution3 = Screen.get_currentResolution();
				Console.Log("wrong resolution found: " + ((object)currentResolution3).ToString());
				Screen.SetResolution(Screen.get_width(), Screen.get_height(), false);
			}
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			string[] array = commandLineArgs;
			foreach (string text in array)
			{
				if (text.CompareTo("-lowres") == 0)
				{
					Screen.SetResolution(800, 600, false);
				}
				if (text.CompareTo("-tinyres") == 0)
				{
					Screen.SetResolution(640, 480, false);
				}
			}
		}
	}
}
