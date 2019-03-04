using Svelto.ServiceLayer;

internal interface ILoadTutorialStageRequest : IServiceRequest, IAnswerOnComplete<LoadTutorialStageData>
{
	void ClearCache();
}
