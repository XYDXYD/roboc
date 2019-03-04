using Svelto.ServiceLayer;

internal interface ILoadPrebuiltRobotColorCombinationsRequest : IServiceRequest, IAnswerOnComplete<PrebuiltRobotColorCombinations>
{
	void ClearCache();
}
