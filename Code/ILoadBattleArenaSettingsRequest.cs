using Svelto.ServiceLayer;

internal interface ILoadBattleArenaSettingsRequest : IServiceRequest, IAnswerOnComplete<BattleArenaSettingsDependency>
{
	void ClearCache();
}
