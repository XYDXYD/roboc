namespace Login
{
	public enum UserValidationResultCode
	{
		UserValid,
		UserInvalid,
		UserNotFound,
		Suspended,
		UnknownError,
		RobocraftAccountNotCreatedYet,
		AccountUnconfirmed,
		AccountPasswordInvalidated,
		AccountBadUsernameOrPassword,
		AccountBlockedTemporarily,
		AccountBlocked
	}
}
