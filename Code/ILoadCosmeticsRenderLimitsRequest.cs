using Svelto.ServiceLayer;

internal interface ILoadCosmeticsRenderLimitsRequest : IServiceRequest, IAnswerOnComplete<CosmeticsRenderLimitsDependency>
{
}
