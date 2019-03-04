using System.Collections.Generic;

namespace Login
{
	public class RegisterNewUserResult
	{
		public RegisterNewUserResultCode ResultCode;

		public string ReasonString;

		public string Username;

		public string DisplayName;

		public string EmailAddress;

		public bool EmailValidated;

		public List<object> Flags;

		public string Token;

		public RegisterNewUserResult(string username_, string emailAddress_, bool emailValidated_, List<object> flags_, string token_)
		{
			Username = username_;
			EmailAddress = emailAddress_;
			EmailValidated = emailValidated_;
			Flags = flags_;
			Token = token_;
		}

		public RegisterNewUserResult(string username_, string token_, string displayName_)
		{
			Username = username_;
			Token = token_;
			DisplayName = displayName_;
		}

		public RegisterNewUserResult(RegisterNewUserResultCode resultCode_)
		{
			ResultCode = resultCode_;
			ReasonString = string.Empty;
		}

		public RegisterNewUserResult(string failReasonString)
		{
			ResultCode = RegisterNewUserResultCode.RegistrationFailedLocalisedStringReason;
			ReasonString = failReasonString;
		}
	}
}
