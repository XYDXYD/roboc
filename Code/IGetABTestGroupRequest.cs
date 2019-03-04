using Svelto.ServiceLayer;

internal interface IGetABTestGroupRequest : IServiceRequest, IAnswerOnComplete<ABTestData>
{
}
