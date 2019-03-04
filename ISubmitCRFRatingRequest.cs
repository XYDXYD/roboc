using Svelto.ServiceLayer;

internal interface ISubmitCRFRatingRequest : IServiceRequest<SubmitCRFRatingDependency>, IAnswerOnComplete, IServiceRequest
{
}
