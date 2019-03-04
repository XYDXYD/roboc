using System.Collections.Generic;

namespace Login
{
	public class UserValidationResult
	{
		public UserValidationResultCode ResultCode;

		public string DisplayName;

		public string PublicID;

		public string LegacyName;

		public string EmailAddress;

		public bool EmailValidated;

		public List<string> Flags;

		public string Token;

		public string ErrorMessage;

		public UserValidationResult(UserValidationResultCode resultCode, string errorMessage)
		{
			ResultCode = resultCode;
			ErrorMessage = errorMessage;
		}

		public UserValidationResult(UserValidationResultCode resultCode_, string legacyName_ = "", string PublicID_ = "", string DisplayName_ = "", string EmailAddress_ = "", bool EmailValidated_ = false, List<string> Flags_ = null, string Token_ = "")
		{
			ResultCode = resultCode_;
			DisplayName = DisplayName_;
			PublicID = PublicID_;
			LegacyName = legacyName_;
			EmailAddress = EmailAddress_;
			EmailValidated = EmailValidated_;
			Flags = Flags_;
			Token = Token_;
		}
	}
}
