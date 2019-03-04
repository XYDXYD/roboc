using Svelto.ServiceLayer;

internal interface ICopyAndConstructRobotFromCRFRequest : IServiceRequest<CopyAndConstructRobotDependency>, IAnswerOnComplete, IServiceRequest
{
}
