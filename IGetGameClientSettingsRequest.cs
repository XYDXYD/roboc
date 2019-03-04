using Svelto.ServiceLayer;

internal interface IGetGameClientSettingsRequest : IServiceRequest, IAnswerOnComplete<GameClientSettingsDependency>
{
}
