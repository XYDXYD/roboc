using Svelto.ServiceLayer;

internal interface ILoadTechPointsRequest : IServiceRequest, IAnswerOnComplete<int>
{
	void ClearCache();
}
