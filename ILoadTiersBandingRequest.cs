using Svelto.ServiceLayer;

internal interface ILoadTiersBandingRequest : IServiceRequest, IAnswerOnComplete<TiersData>
{
}
