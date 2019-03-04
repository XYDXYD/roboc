using Svelto.ServiceLayer;

internal interface ICreatePrebuiltRobotRequest : IServiceRequest<CreatePrebuiltRobotDependency>, IAnswerOnComplete, IServiceRequest
{
}
