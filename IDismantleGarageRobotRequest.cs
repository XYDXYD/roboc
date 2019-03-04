using Svelto.ServiceLayer;

internal interface IDismantleGarageRobotRequest : IServiceRequest<uint>, IAnswerOnComplete, IServiceRequest
{
}
