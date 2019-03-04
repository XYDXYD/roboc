using Svelto.ServiceLayer;

internal interface IGetMegabotRefundInfoRequest : IServiceRequest, IAnswerOnComplete<MegabotRefundInfo>
{
}
