using Svelto.ServiceLayer;

internal interface IGetGameStartAudioRequest : IServiceRequest, IAnswerOnComplete<string>
{
}
