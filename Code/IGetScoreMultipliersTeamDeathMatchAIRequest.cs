using GameServerServiceLayer;
using Svelto.ServiceLayer;

internal interface IGetScoreMultipliersTeamDeathMatchAIRequest : IServiceRequest, IAnswerOnComplete<ScoreMultipliers>
{
}
