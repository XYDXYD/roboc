using Svelto.ServiceLayer;

internal interface ILoadPlayerRoboPassSeasonRequest : IServiceRequest, IAnswerOnComplete<PlayerRoboPassSeasonData>
{
	void ClearCache();
}
