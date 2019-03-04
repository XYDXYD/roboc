using Svelto.ServiceLayer;

internal interface IConstructReadOnlyRobotRequest : IServiceRequest<int>, IAnswerOnComplete, IServiceRequest
{
}
