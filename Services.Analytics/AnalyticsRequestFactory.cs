using LobbyServiceLayer;
using Services.Analytics.Tencent;
using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal class AnalyticsRequestFactory : ServiceRequestFactory, IAnalyticsRequestFactory, IServiceRequestFactory
	{
		public AnalyticsRequestFactory()
		{
			AddRelation<ILogFriendInvitedRequest, LogFriendInvitedRequest_Tencent>();
			AddRelation<ILogPlayerEnteredGameRequest, LogPlayerEnteredGameRequest_Tencent, EnterBattleDependency>();
			AddRelation<ILogPlayerStartedGameRequest, LogPlayerStartedGameRequest_Tencent, LogPlayerStartedGameDependency>();
			AddRelation<ILogPlayerEndedGameRequest, LogPlayerEndedGameRequest_Tencent, LogPlayerEndedGameDependency>();
			AddRelation<ILogPlayerEnteredPracticeGameRequest, LogPlayerEnteredPracticeGameRequest_Tencent>();
			AddRelation<ILogPlayerEnteredCampaignRequest, LogPlayerEnteredCampaignRequest_Tencent>();
			AddRelation<ILogPlayerKillRequest, LogPlayerKillRequest_Tencent, LogPlayerKillDependency>();
			AddRelation<ILogPlayerCreatedNewRobotRequest, LogPlayerCreatedNewRobotRequest_Tencent, CreatedNewRobotDependency>();
			AddRelation<ILogPlayerEditedRobotRequest, LogPlayerEditedRobotRequest_Tencent, EditedRobotDependency>();
			AddRelation<ILogPlayerDismantledRobotRequest, LogPlayerDismantledRobotRequest_Tencent, DismantledRobotDependency>();
			AddRelation<ILogPlayerClickedInRobotShopRequest, LogPlayerClickedInRobotShopRequest_Tencent, int>();
			AddRelation<ILogPlayerCampaignWaveSummaryRequest, LogPlayerCampaignWaveSummaryRequest_Tencent, LogPlayerCampaignWaveSummaryDependency>();
			AddRelation<ILogAskedToReconnectRequest, LogAskedToReconnectRequest_Tencent>();
			AddRelation<ILogChatJoinedRequest, LogChatJoinedRequest_Tencent, ChatChannelType>();
			AddRelation<ILogChatSentRequest, LogChatSentRequest_Tencent, LogChatSentDependency>();
			AddRelation<ILogChatCreatedRequest, LogChatCreatedRequest_Tencent>();
			AddRelation<ILogClaimedPromotionRequest, LogClaimedPromotionRequest_Tencent, LogClaimedPromotionDependency>();
			AddRelation<ILogClanCreatedRequest, LogClanCreatedRequest_Tencent, string>();
			AddRelation<ILogClanJoinedRequest, LogClanJoinedRequest_Tencent, string>();
			AddRelation<ILogClanLeftRequest, LogClanLeftRequest_Tencent, string>();
			AddRelation<ILogCollectedSeasonRewardRequest, LogCollectedSeasonRewardRequest_Tencent, int>();
			AddRelation<ILogCubeUnlockedRequest, LogCubeUnlockedRequest_Tencent, LogCubeUnlockedDependency>();
			AddRelation<ILogErrorRequest, LogErrorRequest_Tencent, string>();
			AddRelation<ILogFrameRateRequest, LogFrameRateRequest_Tencent, LogFrameRateDependency>();
			AddRelation<ILogFriendAcceptInviteRequest, LogFriendAcceptInviteRequest_Tencent>();
			AddRelation<ILogFriendAddedToPartyRequest, LogFriendAddedToPartyRequest_Tencent>();
			AddRelation<ILogLevelUpRequest, LogLevelUpRequest_Tencent, LogLevelUpDependency>();
			AddRelation<ILogLoadingRequest, LogLoadingRequest_Tencent, string>();
			AddRelation<ILogOnLoggedInPlayerDataRequest, LogOnLoggedInPlayerDataRequest_Tencent, LogOnLoggedInPlayerDataDependency>();
			AddRelation<ILogOnLoggedInSettingsRequest, LogOnLoggedInSettingsRequest_Tencent, LogOnLoggedInSettingsDependency>();
			AddRelation<ILogPromoCodeActivatedRequest, LogPromoCodeActivatedRequest_Tencent, LogPromoCodeActivatedDependency>();
			AddRelation<ILogReconnectedRequest, LogReconnectedRequest_Tencent, float>();
			AddRelation<ILogRobotControlsChangedRequest, LogRobotControlsChangedRequest_Tencent, LogRobotControlsChangedDependency>();
			AddRelation<ILogRobotShopCollectedEarningsRequest, LogRobotShopCollectedEarningsRequest_Tencent, LogRobotShopCollectedEarningsDependency>();
			AddRelation<ILogRobotShopDownloadedRequest, LogRobotShopDownloadedRequest_Tencent, LogRobotShopDownloadedDependency>();
			AddRelation<ILogRobotShopUploadedRequest, LogRobotShopUploadedRequest_Tencent, LogRobotShopUploadedDependency>();
			AddRelation<ILogSettingsChangedRequest, LogSettingsChangedRequest_Tencent, LogSettingsChangedDependency>();
			AddRelation<ILogSuccessfulLoginRequest, LogSuccessfulLoginRequest_Tencent, LogSuccessfulLoginDependency>();
			AddRelation<ILogPlayerEnteredMothershipRequest, LogPlayerEnteredMothershipRequest_Tencent, LogPlayerEnteredMothershipDependency>();
			AddRelation<ILogGarageSlotSelectedRequest, LogGarageSlotSelectedRequest_Tencent>();
			AddRelation<ILogPlayerLeftMothershipRequest, LogPlayerLeftMothershipRequest_Tencent, LogPlayerLeftMothershipDependency>();
			AddRelation<ILogRobotNameChangedRequest, LogRobotNameChangedRequest_Tencent>();
			AddRelation<ILogTierRankUpRequest, LogTierRankUpRequest_Tencent, LogTierRankUpDependency>();
			AddRelation<ILogItemBoughtRequest, LogItemBoughtRequest_Tencent, LogItemBoughtDependency>();
			AddRelation<ILogItemStockedRequest, LogItemStockedRequest_Tencent, LogItemStockedDependency>();
			AddRelation<ILogItemShopVisitedRequest, LogItemShopVisitedRequest_Tencent, LogItemShopVisitedDependency>();
			AddRelation<ILogRobotDownloadedRequest, LogRobotDownloadedRequest_Tencent, LogRobotDownloadedDependency>();
			AddRelation<ILogPlayerXpEarnedRequest, LogPlayerXpEarnedRequest_Tencent, LogPlayerXpEarnedDependency>();
			AddRelation<ILogPlayerCurrencyEarnedRequest, LogPlayerCurrencyEarnedRequest_Tencent, LogPlayerCurrencyEarnedDependency>();
			AddRelation<ILogPlayerCurrencySpentRequest, LogPlayerCurrencySpentRequest_Tencent, LogPlayerCurrencySpentDependency>();
			AddRelation<ILogQuestCompletedRequest, LogQuestCompletedRequest_Tencent, LogQuestCompletedDependency>();
			AddRelation<ILogQuestAddedRequest, LogQuestAddedRequest_Tencent, LogQuestAddedDependency>();
			AddRelation<ILogQuestRerolledRequest, LogQuestRerolledRequest_Tencent, LogQuestRerolledDependency>();
			AddRelation<ILogPurchaseFunnelRequest, LogPurchaseFunnelRequest_Tencent, LogPurchaseFunnelDependency>();
			AddRelation<ILogPlayerRoboPassGradeUpRequest, LogPlayerRoboPassGradeUpRequest_Tencent, LogPlayerRoboPassGradeUpDependency>();
			AddRelation<ILogPlayerRoboPassRewardCollectedRequest, LogPlayerRoboPassRewardCollectedRequest_Tencent, LogPlayerRoboPassRewardCollectedDependency>();
		}
	}
}
