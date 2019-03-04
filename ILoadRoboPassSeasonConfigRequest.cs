using Svelto.ServiceLayer;

internal interface ILoadRoboPassSeasonConfigRequest : IServiceRequest, IAnswerOnComplete<RoboPassSeasonData>
{
	void ClearCache();
}
