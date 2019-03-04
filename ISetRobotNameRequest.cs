using Svelto.ServiceLayer;

internal interface ISetRobotNameRequest : IServiceRequest<SetRobotNameDependency>, IAnswerOnComplete<string>, IServiceRequest
{
}
