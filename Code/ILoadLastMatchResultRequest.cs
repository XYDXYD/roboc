using Svelto.ServiceLayer;

internal interface ILoadLastMatchResultRequest : IServiceRequest, IAnswerOnComplete<LastMatchResultData>
{
}
