using Svelto.ServiceLayer;

internal interface ILoadPlayerDataRequest : IServiceRequest, IAnswerOnComplete<PlayerDataResponse>
{
}
