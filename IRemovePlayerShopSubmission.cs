using Svelto.ServiceLayer;

internal interface IRemovePlayerShopSubmission : IServiceRequest<int>, IAnswerOnComplete<LoadShopEarningsResponse>, IServiceRequest
{
}
