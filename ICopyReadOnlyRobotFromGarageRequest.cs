using Svelto.ServiceLayer;

internal interface ICopyReadOnlyRobotFromGarageRequest : IServiceRequest<int>, IAnswerOnComplete, IServiceRequest
{
}
