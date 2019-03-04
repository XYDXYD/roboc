using Svelto.ServiceLayer;

internal interface ICheckPromotionsRequest : IServiceRequest<string>, IAnswerOnComplete<ClaimPendingPromotionsResponse>, IServiceRequest
{
}
