using Svelto.ServiceLayer;

internal interface IBuyGarageSlotRequest : IServiceRequest, IAnswerOnComplete<BuyGarageSlotResponse>
{
}
