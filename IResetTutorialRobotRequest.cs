using Svelto.ServiceLayer;

internal interface IResetTutorialRobotRequest : IServiceRequest<ResetTutorialRobotDependancy>, IAnswerOnComplete<uint>, IServiceRequest
{
}
