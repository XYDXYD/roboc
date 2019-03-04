namespace Services.Web.Photon
{
	public enum WebServicesErrorCode : short
	{
		None = 0,
		CPUTooHigh = 1,
		CPUTooLow = 2,
		RobotTierNotAllowed = 3,
		CubeIDNotAllowed = 4,
		CubeTypeNotAllowed = 5,
		DatabaseError = 8,
		UnexpectedError = 9,
		WrongNumberOfAuthParams = 10,
		Banned = 11,
		EACValidationFailed = 12,
		NotSteamUser = 13,
		PromotionDoesntExist = 14,
		NotEnoughMoney = 17,
		MaxGarageSlots = 18,
		PlatformFeatureNotAvailable = 19,
		UserDoesNotHaveAllCubeTypes = 20,
		ReplaceDailyQuestLimit = 21,
		MaintenanceModeError = 125,
		RobotShopMaintenanceMode = 126,
		InvalidRobot = 140,
		ExpiredRobot = 144,
		RobotHasSanction = 145,
		CustomisationNotOwned = 146,
		ItemShopBundleExpired = 147,
		UserNotFound = 200,
		InvalidUsernameFormat = 201,
		UsernameTooLong = 202,
		InvalidUsername = 203,
		TencentValidationFail = 204,
		UsernameAlreadyTaken = 205,
		UsernameTooShort = 206,
		SaleEnded = 207
	}
}
