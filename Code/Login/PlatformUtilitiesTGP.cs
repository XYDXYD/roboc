using Services.Web.Photon.Tencent;
using Svelto.IoC;
using System;
using System.Collections;
using Utility;

namespace Login
{
	public class PlatformUtilitiesTGP : IPlatformUtilities
	{
		[Inject]
		internal ILoginWebservicesRequestFactory requestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ProfanityFilter profanityFilter
		{
			get;
			set;
		}

		public IEnumerator ReadyPlatform()
		{
			Console.Log("call RailManager_Tencent.RailReady()");
			if (!RailManager_Tencent.RailReady())
			{
				yield break;
			}
			while (true)
			{
				Console.Log("call RailManager_Tencent.SessionReady()");
				if (!RailManager_Tencent.SessionReady())
				{
					yield return null;
					continue;
				}
				break;
			}
		}

		public IEnumerator AuthenticateExistingUser()
		{
			Console.Log("call AuthenticateExistingUser()");
			IRailIDLoginRequest_Tencent loginRequest = requestFactory.Create<IRailIDLoginRequest_Tencent, RailIDLoginDependency>(new RailIDLoginDependency(RailManager_Tencent.RailID, RailManager_Tencent.RailSessionID));
			TaskService<RailIDLoginResponse> loginTask = new TaskService<RailIDLoginResponse>(loginRequest);
			yield return loginTask;
			if (loginTask.succeeded)
			{
				yield return new UserValidationResult(UserValidationResultCode.UserValid, loginTask.result.legacyName, string.Empty, loginTask.result.displayName, string.Empty, EmailValidated_: false, null, loginTask.result.authToken);
			}
			else
			{
				yield return new UserValidationResult(UserValidationResultCode.UserInvalid, loginTask.behaviour.errorBody);
			}
		}

		public IEnumerator AuthenticateExistingUser(string identifier, string password_)
		{
			throw new Exception("not used in TGP flow");
		}

		public IEnumerator PreAuthenticate()
		{
			Console.Log("call PreAuthenticate()  with rail ID " + RailManager_Tencent.RailID + " and session ID" + RailManager_Tencent.RailSessionID);
			IIsPlayerRegisteredRequest_Tencent request = requestFactory.Create<IIsPlayerRegisteredRequest_Tencent, IsPlayerRegisteredDependency>(new IsPlayerRegisteredDependency(RailManager_Tencent.RailID, RailManager_Tencent.RailSessionID));
			TaskService<bool> task = new TaskService<bool>(request);
			yield return task;
			if (task.succeeded)
			{
				if (task.result)
				{
					yield return PreAuthenticateResult.UserFound;
				}
				else
				{
					yield return PreAuthenticateResult.UserNotFound;
				}
				yield break;
			}
			throw new Exception(task.behaviour.errorBody);
		}

		public bool VerifyPlatformReadyness()
		{
			Console.Log("call VerifyPlatformReadyness() - waiting for the TGP session callback..");
			if (!RailManager_Tencent.SessionReady())
			{
				return false;
			}
			return true;
		}

		public IEnumerator RegisterNewUser(RegisterNewUserDependency registerUserDependency)
		{
			if (profanityFilter.FilterString(registerUserDependency.DisplayName) != registerUserDependency.DisplayName)
			{
				yield return new RegisterNewUserResult(RegisterNewUserResultCode.UsernameProfanity);
				yield break;
			}
			IRegisterUserRequest_Tencent request = requestFactory.Create<IRegisterUserRequest_Tencent, RegisterUserDependency>(new RegisterUserDependency(RailManager_Tencent.RailID, RailManager_Tencent.RailSessionID, registerUserDependency.DisplayName));
			TaskService<RegisterUserReturn> task = new TaskService<RegisterUserReturn>(request);
			yield return task;
			if (task.succeeded)
			{
				RegisterUserReturn requestResult = task.result;
				yield return new RegisterNewUserResult(requestResult.UserName, requestResult.Token, requestResult.DisplayName);
				yield break;
			}
			RegisterNewUserResultCode resultCode = RegisterNewUserResultCode.RegistrationFailedLocalisedStringReason;
			switch (task.behaviour.errorCode)
			{
			case 201:
				resultCode = RegisterNewUserResultCode.UsernameNotPermitted;
				break;
			case 203:
				resultCode = RegisterNewUserResultCode.InvalidUsername;
				break;
			case 202:
				resultCode = RegisterNewUserResultCode.UsernameTooLong;
				break;
			case 204:
				resultCode = RegisterNewUserResultCode.ValidationFail;
				break;
			case 205:
				resultCode = RegisterNewUserResultCode.UsernameAlreadyTaken;
				break;
			case 206:
				resultCode = RegisterNewUserResultCode.UsernameTooShort;
				break;
			}
			yield return new RegisterNewUserResult(resultCode);
		}

		public IEnumerator ChangeDisplayName(string newName)
		{
			yield return UsernameValidDuringEntryStatus.EntryIsValid;
		}

		public IEnumerator CheckDisplayNameChangeFlag()
		{
			yield return CheckDisplayNameChangeFlagResult.NoDisplayNameChange;
		}

		public IEnumerator CheckIdentifierAvailableDuringEntry(string identifier)
		{
			if (profanityFilter.FilterString(identifier) != identifier)
			{
				yield return UsernameValidDuringEntryStatus.EntryContainsProfanity;
			}
			yield return UsernameValidDuringEntryStatus.EntryIsValid;
		}
	}
}
