using Svelto.ServiceLayer;

internal interface IDeleteGarageRequest : IServiceRequest<uint>, IAnswerOnComplete, IServiceRequest
{
}
