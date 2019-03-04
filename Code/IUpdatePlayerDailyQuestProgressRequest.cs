using Simulation;
using Svelto.ServiceLayer;

public interface IUpdatePlayerDailyQuestProgressRequest : IServiceRequest<LocalPlayerDailyQuestProgress>, IAnswerOnComplete, IServiceRequest
{
}
