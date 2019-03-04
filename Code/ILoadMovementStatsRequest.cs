using Svelto.ServiceLayer;

internal interface ILoadMovementStatsRequest : IServiceRequest, IAnswerOnComplete<MovementStats>
{
	void ClearCache();
}
