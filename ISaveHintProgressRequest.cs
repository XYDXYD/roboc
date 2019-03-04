using Svelto.ServiceLayer;

internal interface ISaveHintProgressRequest : IServiceRequest<byte[]>, IAnswerOnComplete<byte[]>, IServiceRequest
{
}
