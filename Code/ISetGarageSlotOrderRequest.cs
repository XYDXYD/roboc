using Svelto.ServiceLayer;

internal interface ISetGarageSlotOrderRequest : IServiceRequest<SetGarageSlotOrderDependency>, IAnswerOnComplete<SetGarageSlotOrderDependency>, IServiceRequest
{
}
