using Svelto.ServiceLayer;

internal interface ISavePlayerBuildingXPRequest : IServiceRequest, IAnswerOnComplete<PlayerLevelAndXPData>
{
}
