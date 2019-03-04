namespace Services.Web.Photon
{
	public enum WebServicesParameterCode : byte
	{
		Guid = 0,
		StaticData = 1,
		Text = 2,
		SessionId = 3,
		ApplyPromoCodeRoboPass = 4,
		RobitsCost = 5,
		CosmeticCreditsCost = 6,
		UNUSED_7 = 7,
		IntNumber = 8,
		Reason = 9,
		IsModerator = 10,
		IsDeveloper = 11,
		IsAdmin = 12,
		IntHours = 13,
		IntMinutes = 14,
		IntSeconds = 0xF,
		CubeInventoryData = 0x10,
		ChatSettingsData = 17,
		RobotMasteryLevel = 18,
		MaintenanceModeMessage = 19,
		MaintenanceModeFlag = 20,
		DailyQuestProgress = 21,
		CampaignID = 22,
		CampaignDifficulty = 23,
		UserName = 30,
		GarageSlot = 0x1F,
		ItemShopBundleId = 0x20,
		RobotColorData = 33,
		DefaultPaletteData = 34,
		RobotShopId = 35,
		GameClientSettings = 36,
		AutoRegenHealthSettings = 37,
		UNUSED_38 = 38,
		UNUSED_39 = 39,
		CompositeIdPart1 = 40,
		CompositeIdPart2 = 41,
		RobotName = 42,
		GarageSlotId = 43,
		GarageSlotsData = 44,
		RobotId = 45,
		RobotRawData = 46,
		RobotSaveResult = 47,
		CurrentSlotId = 48,
		CurrentRobotBinaryData = 49,
		UNUSED_50 = 50,
		CurrentRobotCubesNumber = 51,
		CurrentRobotWeaponCategory = 52,
		ItemShopCurrencyType = 53,
		RobotUniqueId = 54,
		ThumbnailVersion = 55,
		CurrentRobotMovementList = 56,
		WeaponStats = 57,
		GarageSlotOrder = 58,
		GarageSlotControlType = 59,
		GarageSlotControlOptions = 60,
		PowerBarSettingsData = 61,
		SteamId = 62,
		PromotionList = 0x3F,
		CurrentCampaignsData = 0x40,
		ItemShopData = 65,
		TierProgress = 66,
		PartyRobotTiersStatus = 67,
		GarageSlotLimit = 68,
		CampaignsConfigData = 69,
		LiveCampaignsWavesData = 70,
		SignUpDate = 71,
		CubesRequestData = 72,
		CampaignWaveNo = 73,
		RpBalance = 74,
		CurrentSPCampaignWavesData = 75,
		TierProgressNotification = 76,
		QuestID = 77,
		MapName = 78,
		BuildingXPSettings = 79,
		UNUSED_80 = 80,
		PlayerLevel = 81,
		CampaignGameResult = 82,
		UNUSED_83 = 83,
		CampaignLongPlayMultiplier = 84,
		RobotRanking = 84,
		GameGUID = 85,
		CosmeticCreditsAmount = 86,
		CcBalance = 87,
		PurchaseRequestData = 88,
		LastCompletedCampaignFound = 89,
		UNUSED_89 = 89,
		RobotShopItem = 90,
		EacToken = 91,
		RobotShopSearchCriteria = 92,
		RobotShopItemList = 93,
		RobotShopItemId = 94,
		RobotShopItemData = 95,
		RobotShopEarnings = 96,
		CombatRating = 97,
		CosmeticRating = 98,
		ClientBuildName = 99,
		PlayerSubmissionInfo = 100,
		RobotData = 101,
		RobotSanctions = 102,
		RobotUploadResult = 103,
		CubesOffsetX = 104,
		CubesOffsetZ = 105,
		ExpectedFirstCubeLocationX = 106,
		ExpectedFirstCubeLocationY = 107,
		ExpectedFirstCubeLocationZ = 108,
		GameModeTypesPerLobby = 109,
		PurchaseCrfRobotData = 110,
		ValidateRobotForPlayResult = 111,
		MinimumGameVersion = 112,
		ValidateUserFoundObsoleteCubes = 113,
		ValidateUserFoundNotOwnedCubes = 114,
		ValidateUserSpecialRewardTitle = 115,
		ValidateUserSpecialRewardBody = 116,
		ValidateUserRefundedObsoleteCubes = 117,
		ValidateUserFoundReplacementCubes = 118,
		ValidateUserSendIsNewUserToClient = 119,
		GetIfPlayerIsModeratorParam = 120,
		SteamPromotionIds = 121,
		PromoCode = 122,
		ApplyPromoCodeResult = 123,
		ApplyPromoCodeResultCode = 124,
		ApplyPromoCodeIsSerialKey = 125,
		ApplyPromoCodeValue = 126,
		ApplyPromoCodeId = 0x7F,
		ApplyPromoCodeCubesAwarded = 0x80,
		AvatarId = 129,
		UseCustomAvatar = 130,
		CustomAvatarBytes = 131,
		CustomAvatarFormat = 132,
		ApplyPromoCodeMessageStrKey = 133,
		LobbyType = 134,
		PlayerStartedPurchase = 135,
		GameModeType = 136,
		GameScoreMultipliers = 137,
		DefaultWeaponOrderSubcategories = 138,
		TutorialInProgress = 140,
		TutorialCompleted = 141,
		TutorialSkipped = 142,
		TutorialStage = 143,
		UNUSED_144 = 144,
		UNUSED_145 = 145,
		ResetTutorialRobotRequestResult = 146,
		UNUSED_147 = 147,
		ResetRobotStage = 148,
		RobotColorPaletteOrder = 149,
		HasPremiumForLife = 150,
		NumDaysPremiumActuallyAwarded = 151,
		BrawlParametersData = 152,
		BrawlDetailsData = 153,
		ValidateRobotForBrawlReturnResult = 154,
		PlayerActiveDailyQuests = 155,
		BrawlEventChangeLockedStatus = 156,
		BrawlCubeList = 157,
		BrawlWeaponStats = 158,
		BrawlPowerBarSettings = 159,
		BrawlAutoRegenHealthSettings = 160,
		BrawlLanguagePrefix = 161,
		BrawlLanguageStrings = 162,
		BrawlFirstVictoryBonus = 163,
		DesiredBrawlNumberForFirstVictory = 164,
		UNUSED_165 = 165,
		ABTest = 166,
		ABTestGroup = 167,
		CustomGameRequestResponse = 168,
		CustomGameSessionData = 169,
		CustomGameFetchMapsListData = 170,
		CustomGameInviteTarget = 171,
		CustomGameEventSessionInfo = 172,
		CustomGameReplyToInvitationAcceptOrDeclineChoice = 173,
		UNUSED_174 = 174,
		CustomGameInviteIsTeamA = 175,
		RobotCosmeticCPU = 176,
		RobotCPU = 177,
		CustomGameFetchMapsLanguageStrings = 178,
		CustomGameSessionFieldToChange = 179,
		CustomGameSessionFieldNewValue = 180,
		CustomGameConfigChangeEventData = 181,
		CustomGameLeaderChangedEventData = 182,
		CustomGameKickFromSessionTarget = 183,
		CustomGameKickFromSessionEventData = 184,
		CustomGameTeamAssignmentChangeSourceParameter = 185,
		CustomGameTeamAssignmentChangeTargetParameter = 186,
		CustomGameTeamAssignmentDestinationIsTeamB = 187,
		CustomGamePlayerStateChange = 188,
		CustomGameHasInviteData = 189,
		CustomGameDeclineInvitationEventData = 190,
		IsCustomGameOverride = 191,
		DamageBoostData = 192,
		RobotMasterySettingsData = 193,
		CpuSettingsData = 194,
		TauntsData = 195,
		CosmeticsRenderLimitsData = 196,
		PlatformConfigData = 197,
		UNUSED_198 = 198,
		PlayerCurrentProgress = 199,
		UNUSED_200 = 200,
		UNUSED_201 = 201,
		UNUSED_202 = 202,
		VoteThresholds = 203,
		UNUSED_204 = 204,
		PlayerGainedXP = 205,
		IsGameReconnectable = 207,
		ApplyPromoCodeBundleId = 208,
		UNUSED_209 = 209,
		TechTreeData = 210,
		TechTreeNodeID = 211,
		TechPointsAwards = 212,
		CopyRobotExtension = 213,
		TpBalance = 214,
		GameModePreferences = 215,
		AwardedCubeIds = 216,
		RailID = 217,
		UserAuthToken = 218,
		RailSessionID = 219,
		IsPlayerRegistered = 220,
		ProductSKU = 221,
		RailOrderID = 222,
		OrderCompleted = 223,
		UserPassword = 224,
		BattleData = 225,
		ClickCount = 226,
		CampaignWaveSummaryData = 227,
		SkinCustomisationsList = 228,
		SpawnFXCustomisationsList = 229,
		DeathFXCustomisationsList = 230,
		OwnedSkinCustomisationsList = 231,
		OwnedSpawnCustomisationsList = 232,
		OwnedDeathCustomisationsList = 233,
		BaySkinId = 234,
		SpawnEffectID = 235,
		DeathEffectID = 236,
		PlayerRoboPassSeasonData = 237
	}
}