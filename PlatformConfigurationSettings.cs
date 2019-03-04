internal class PlatformConfigurationSettings
{
	public readonly bool BuyPremiumAvailable;

	public readonly bool MainShopButtonAvailable;

	public readonly bool RoboPassButtonAvailable;

	public readonly bool LanguageSelectionAvailable;

	public readonly bool CanCreateChatRooms;

	public readonly bool IsCurseVoiceEnabled;

	public readonly bool IsDeltaDNAEnabled;

	public readonly string AutoJoinPublicChatRoom;

	public readonly bool UseDecimalSystem;

	public readonly string FeedbackURL;

	public readonly string SupportURL;

	public readonly string WikiURL;

	public PlatformConfigurationSettings(bool BuyPremiumAvailable_, bool MainShopButtonAvailable_, bool RoboPassButtonAvailable_, bool LanguageSelectionAvailable_, string AutoJoinPublicChatRoom_, bool CanCreateChatRooms_, bool IsCurseVoiceEnabled_, bool IsDeltaDNAEnabled_, bool UseDecimalSystem_, string feedbackURL_, string supportURL_, string wikiURL_)
	{
		BuyPremiumAvailable = BuyPremiumAvailable_;
		MainShopButtonAvailable = MainShopButtonAvailable_;
		RoboPassButtonAvailable = RoboPassButtonAvailable_;
		LanguageSelectionAvailable = LanguageSelectionAvailable_;
		AutoJoinPublicChatRoom = AutoJoinPublicChatRoom_;
		CanCreateChatRooms = CanCreateChatRooms_;
		IsCurseVoiceEnabled = IsCurseVoiceEnabled_;
		IsDeltaDNAEnabled = IsDeltaDNAEnabled_;
		UseDecimalSystem = UseDecimalSystem_;
		FeedbackURL = feedbackURL_;
		SupportURL = supportURL_;
		WikiURL = wikiURL_;
	}
}
