using Svelto.ServiceLayer;

internal interface IReplaceDailyQuestRequest : IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
{
}
