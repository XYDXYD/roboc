using Svelto.ServiceLayer;

internal interface ILoadIfPlayerIsModeratorRequest : IServiceRequest, IAnswerOnComplete<string>
{
}
