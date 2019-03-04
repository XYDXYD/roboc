using Svelto.ServiceLayer;

internal interface ILoadPrebuiltRobotDataRequest : IServiceRequest, IAnswerOnComplete<PrebuiltRobotsDependency>
{
	void ClearCache();
}
