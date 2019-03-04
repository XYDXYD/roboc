using Svelto.ServiceLayer;

internal interface ILoadGarageDataRequest : IServiceRequest, IAnswerOnComplete<LoadGarageDataRequestResponse>
{
}
