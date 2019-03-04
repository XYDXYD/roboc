using Svelto.ServiceLayer;

internal interface ILoadRobotMasterySettingsRequest : IServiceRequest, IAnswerOnComplete<RobotMasterySettingsDependency>
{
	void ClearCache();
}
