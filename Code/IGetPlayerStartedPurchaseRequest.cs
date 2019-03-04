using Svelto.ServiceLayer;

internal interface IGetPlayerStartedPurchaseRequest : IServiceRequest, IAnswerOnComplete<bool>
{
}
