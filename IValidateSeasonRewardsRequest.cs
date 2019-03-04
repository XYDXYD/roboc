using Svelto.ServiceLayer;

internal interface IValidateSeasonRewardsRequest : IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
{
}
