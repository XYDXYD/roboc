using Svelto.ServiceLayer;

internal interface ISetRobotInGameRequest : IServiceRequest<float>, IAnswerOnComplete<bool>, IServiceRequest
{
}
