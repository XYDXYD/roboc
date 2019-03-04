using Authentication;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections;
using Utility;

namespace Login
{
	internal class LoginSequenceTGP : LoginSequenceBase
	{
		private enum LoginSequenceStage
		{
			ChineseWarning,
			WaitChineseWarningFinished,
			IntroVideo,
			WaitVideoFinished,
			LoadConfigData,
			PreCheckMaintenanceMode,
			ReadyPlatform,
			PreAuthenticate,
			EnterDisplaynameForTGPDialog,
			TryRegisterUserForTGP,
			TryAuthenticateExistingUser,
			PostLoginSuccess,
			WaitingSharedLoginFinalise,
			Finished
		}

		private string _displayName;

		private string _authtoken;

		private string _legacyuserName;

		private LoginSequenceStage _currentStage;

		[Inject]
		internal IPlatformUtilities platformUtilities
		{
			private get;
			set;
		}

		public unsafe override void Initialise(GenericLoginController loginController_)
		{
			base.Initialise(loginController_);
			base.introAnimationsSequenceEventObserver.AddAction(new ObserverAction<IntroAnimationsSequenceEventCode>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			SetNextStage(LoginSequenceStage.ChineseWarning);
			loginController = loginController_;
			_analyticsLoginType = AnalyticsLoginType.RailID;
			_analyticsLaunchMode = AnalyticsLaunchMode.Tencent;
		}

		public override bool CheckFinished()
		{
			return _currentStage == LoginSequenceStage.Finished;
		}

		public override bool CheckVideoFinished()
		{
			return _currentStage == LoginSequenceStage.WaitVideoFinished;
		}

		public override void AdvanceToNextStage()
		{
			if (_currentStage < LoginSequenceStage.Finished)
			{
				_currentStage++;
			}
		}

		protected override void FinaliseSharedLoginSequence()
		{
			SetNextStage(LoginSequenceStage.Finished);
		}

		public override IEnumerator UpdateState()
		{
			switch (_currentStage)
			{
			case LoginSequenceStage.Finished:
				yield break;
			case LoginSequenceStage.ChineseWarning:
			{
				SplashLoginGUIMessage messageType2 = new SplashLoginGUIMessage(SplashLoginGUIMessageType.ShowImage);
				loginController.BroadcastGUIMessage(messageType2);
				SetNextStage(LoginSequenceStage.WaitChineseWarningFinished);
				break;
			}
			case LoginSequenceStage.IntroVideo:
			{
				SetNextStage(LoginSequenceStage.WaitVideoFinished);
				SplashLoginGUIMessage messageType = new SplashLoginGUIMessage(SplashLoginGUIMessageType.PlayVideo);
				loginController.BroadcastGUIMessage(messageType);
				break;
			}
			case LoginSequenceStage.LoadConfigData:
				yield return LoadConfigData();
				SetNextStage(LoginSequenceStage.PreCheckMaintenanceMode);
				break;
			case LoginSequenceStage.PreCheckMaintenanceMode:
			{
				MaintenanceModeState maintenanceModeState = CheckMaintenanceMode();
				if (maintenanceModeState.IsInMaintenanceMode)
				{
					base.loadingIconPresenter.NotifyLoadingDone("LoadingSplashScreenData");
					string bodyMessage = maintenanceModeState.MaintenanceModeMessage;
					if (maintenanceModeState.MaintenanceModeMessage == string.Empty)
					{
						bodyMessage = StringTableBase<StringTable>.Instance.GetString("strGenericErrorRetry");
					}
					loginController.RaiseFatalErrorDialog(StringTableBase<StringTable>.Instance.GetString("strLoginFailure"), bodyMessage);
				}
				else
				{
					SetNextStage(LoginSequenceStage.ReadyPlatform);
				}
				break;
			}
			case LoginSequenceStage.ReadyPlatform:
				yield return platformUtilities.ReadyPlatform();
				if (platformUtilities.VerifyPlatformReadyness())
				{
					SetNextStage(LoginSequenceStage.PreAuthenticate);
				}
				else
				{
					loginController.RaiseFatalErrorDialog(StringTableBase<StringTable>.Instance.GetString("strLoginFailure"), StringTableBase<StringTable>.Instance.GetString("strTGPPlatformNotAvailable"));
				}
				break;
			case LoginSequenceStage.PreAuthenticate:
			{
				IEnumerator enumerator = platformUtilities.PreAuthenticate();
				yield return enumerator;
				if (enumerator.Current != null)
				{
					if ((PreAuthenticateResult)enumerator.Current == PreAuthenticateResult.UserFound)
					{
						Console.Log("User Account was found");
						SetNextStage(LoginSequenceStage.TryAuthenticateExistingUser);
						break;
					}
					Console.Log("User Account was not found, switch to register user state");
					base.loadingIconPresenter.NotifyLoadingDone("LoadingSplashScreenData");
					loginController.RaiseNewDialog(typeof(EnterUsernameDialogController));
					SetNextStage(LoginSequenceStage.EnterDisplaynameForTGPDialog);
				}
				break;
			}
			case LoginSequenceStage.TryRegisterUserForTGP:
			{
				base.loadingIconPresenter.ChangeLoadingIconText(StringTableBase<StringTable>.Instance.GetString("strLoggingIn"));
				base.loadingIconPresenter.NotifyLoading("LoadingSplashScreenData");
				Console.Log("Trying to register a new user for this rail account, with the display name: " + _displayName);
				IEnumerator enumerator2 = platformUtilities.RegisterNewUser(new RegisterNewUserDependency(_displayName));
				yield return enumerator2;
				if (enumerator2.Current != null)
				{
					RegisterNewUserResult registerNewUserResult = (RegisterNewUserResult)enumerator2.Current;
					if (registerNewUserResult.ResultCode == RegisterNewUserResultCode.NewUserWasRegisteredSuccesfully)
					{
						Console.Log("New User Account was registered succesfully.");
						_authtoken = registerNewUserResult.Token;
						_displayName = registerNewUserResult.DisplayName;
						_legacyuserName = registerNewUserResult.Username;
						SetNextStage(LoginSequenceStage.PostLoginSuccess);
					}
					else if (registerNewUserResult.ResultCode == RegisterNewUserResultCode.UsernameTooShort)
					{
						ReEnterUserNameWithInfo(StringTableBase<StringTable>.Instance.GetString("strTencentUsernameTooShort"));
					}
					else if (registerNewUserResult.ResultCode == RegisterNewUserResultCode.UsernameProfanity)
					{
						ReEnterUserNameWithInfo(StringTableBase<StringTable>.Instance.GetString("strTencentUsernameProfanity"));
					}
					else if (registerNewUserResult.ResultCode == RegisterNewUserResultCode.UsernameAlreadyTaken)
					{
						ReEnterUserNameWithInfo(StringTableBase<StringTable>.Instance.GetString("strTencentUsernameInUse"));
					}
					else if (registerNewUserResult.ResultCode == RegisterNewUserResultCode.UsernameNotPermitted)
					{
						ReEnterUserNameWithInfo(StringTableBase<StringTable>.Instance.GetString("strTencentInvalidUsernameFormat"));
					}
					else if (registerNewUserResult.ResultCode == RegisterNewUserResultCode.InvalidUsername)
					{
						ReEnterUserNameWithInfo(StringTableBase<StringTable>.Instance.GetString("strTencentInvalidUsername"));
					}
					else if (registerNewUserResult.ResultCode == RegisterNewUserResultCode.UsernameTooLong)
					{
						ReEnterUserNameWithInfo(StringTableBase<StringTable>.Instance.GetString("strTencentUsernameTooLong"));
					}
					else if (registerNewUserResult.ResultCode == RegisterNewUserResultCode.ValidationFail)
					{
						ReEnterUserNameWithInfo(StringTableBase<StringTable>.Instance.GetString("strTencentValidationFail"));
					}
					else if (registerNewUserResult.ResultCode == RegisterNewUserResultCode.RegistrationFailedLocalisedStringReason)
					{
						base.loadingIconPresenter.NotifyLoadingDone("LoadingSplashScreenData");
						loginController.RaiseFatalErrorDialog(StringTableBase<StringTable>.Instance.GetString("strLoginFailure"), registerNewUserResult.ReasonString);
					}
				}
				break;
			}
			case LoginSequenceStage.TryAuthenticateExistingUser:
			{
				base.loadingIconPresenter.ChangeLoadingIconText(StringTableBase<StringTable>.Instance.GetString("strLoggingIn"));
				Console.Log("Trying to validate the existing user: ");
				IEnumerator enumerator3 = platformUtilities.AuthenticateExistingUser();
				yield return enumerator3;
				if (enumerator3.Current != null)
				{
					UserValidationResult userValidationResult = (UserValidationResult)enumerator3.Current;
					if (userValidationResult.ResultCode != 0)
					{
						base.loadingIconPresenter.NotifyLoadingDone("LoadingSplashScreenData");
						Console.Log("TryValidateUser: validation failed");
						throw new Exception("Authentication failed");
					}
					_displayName = userValidationResult.DisplayName;
					_authtoken = userValidationResult.Token;
					_legacyuserName = userValidationResult.LegacyName;
					Console.Log("User validated succesfully. legacy User Name: " + _legacyuserName + " display Name:" + _displayName);
					SetNextStage(LoginSequenceStage.PostLoginSuccess);
				}
				break;
			}
			case LoginSequenceStage.PostLoginSuccess:
				User.InitializeTencent(_authtoken, _legacyuserName, _displayName);
				StartSharedLoginSequence();
				SetNextStage(LoginSequenceStage.WaitingSharedLoginFinalise);
				break;
			case LoginSequenceStage.WaitingSharedLoginFinalise:
				Console.Log("WaitingSharedLoginFinalise");
				break;
			}
			yield return null;
		}

		private bool CheckChineseWarningFinished()
		{
			return _currentStage == LoginSequenceStage.WaitChineseWarningFinished;
		}

		private void ReEnterUserNameWithInfo(string errorMessage)
		{
			base.loadingIconPresenter.NotifyLoadingDone("LoadingSplashScreenData");
			loginController.RaiseNewDialog(typeof(EnterUsernameDialogController));
			loginController.RaiseInformationDialog(StringTableBase<StringTable>.Instance.GetString("strLoginFailure"), errorMessage);
			SetNextStage(LoginSequenceStage.EnterDisplaynameForTGPDialog);
		}

		private void HandleChineseWarningEventOccured(ref IntroAnimationsSequenceEventCode eventCode)
		{
			if (CheckChineseWarningFinished() && eventCode == IntroAnimationsSequenceEventCode.ImageHasFinishedShowing)
			{
				AdvanceToNextStage();
			}
		}

		private void SetNextStage(LoginSequenceStage nextStage)
		{
			_currentStage = nextStage;
			Console.Log("Login sequence, advance to stage: " + _currentStage);
		}

		public override bool UserInputOccured(SplashLoginGUIMessageType inputType)
		{
			LoginSequenceStage currentStage = _currentStage;
			if (currentStage == LoginSequenceStage.EnterDisplaynameForTGPDialog && inputType == SplashLoginGUIMessageType.UsernameSubmitted)
			{
				_displayName = (string)loginController.GetDialogEntry(SplashLoginEntryField.Field_Identifier);
				Console.Log("Login sequence, try to register with the display name:" + _displayName);
				loginController.CloseCurrentDialog();
				SetNextStage(LoginSequenceStage.TryRegisterUserForTGP);
				return true;
			}
			return false;
		}
	}
}
