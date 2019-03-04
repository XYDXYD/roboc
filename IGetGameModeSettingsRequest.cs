using Svelto.ServiceLayer;

internal interface IGetGameModeSettingsRequest : IServiceRequest, IAnswerOnComplete<GameModeSettingsDependency>
{
}
