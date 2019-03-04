using Svelto.ServiceLayer;

internal interface ILoadGarageSlotLimitRequest : IServiceRequest, IAnswerOnComplete<uint>
{
}
