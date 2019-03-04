using Svelto.ServiceLayer;

internal interface ICheckCubeRewardsRequest : IServiceRequest, IAnswerOnComplete<string[]>
{
}
