using Svelto.ServiceLayer;

internal interface IGetBuildingXPSettingsRequest : IServiceRequest, IAnswerOnComplete<BuildingXPSettingsDependency>
{
}
