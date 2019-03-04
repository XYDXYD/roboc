using Svelto.ServiceLayer;

internal interface ISetSelectedRobotRequest : IServiceRequest<SetSelectedRobotDependency>, IAnswerOnComplete<SetSelectedRobotDependency>, IServiceRequest
{
}
