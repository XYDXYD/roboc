namespace Login
{
	public enum RegisterNewUserResultCode
	{
		NewUserWasRegisteredSuccesfully = 0,
		UsernameNotPermitted = 1,
		UsernameAlreadyTaken = 2,
		InvalidUsername = 4,
		UsernameTooLong = 5,
		ValidationFail = 6,
		RegistrationFailedLocalisedStringReason = 7,
		UsernameProfanity = 8,
		UsernameTooShort = 9
	}
}
