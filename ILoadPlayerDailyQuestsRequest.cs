using Svelto.ServiceLayer;

internal interface ILoadPlayerDailyQuestsRequest : IServiceRequest, IAnswerOnComplete<PlayerDailyQuestsData>
{
}
