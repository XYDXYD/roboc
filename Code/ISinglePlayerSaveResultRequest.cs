using Svelto.ServiceLayer;

internal interface ISinglePlayerSaveResultRequest : IServiceRequest<SaveGameAwardsRequestDependency>, IAnswerOnComplete, IServiceRequest
{
}
