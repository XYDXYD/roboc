using Svelto.ServiceLayer;

internal interface ISaveLastMatchResultRequest : IServiceRequest<bool>, IAnswerOnComplete, IServiceRequest
{
}
