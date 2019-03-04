using Svelto.ServiceLayer;

internal interface IUpdateTutorialStageRequest : IServiceRequest<UpdateTutorialStageData>, IAnswerOnComplete<bool>, IServiceRequest
{
}
