using Svelto.ServiceLayer;

internal interface IClaimShopEarningsRequest : IServiceRequest, IAnswerOnComplete<LoadShopEarningsResponse>
{
}
