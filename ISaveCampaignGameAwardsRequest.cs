using Svelto.ServiceLayer;

internal interface ISaveCampaignGameAwardsRequest : IServiceRequest<SaveGameAwardsRequestDependency>, IAnswerOnComplete, IServiceRequest
{
}
