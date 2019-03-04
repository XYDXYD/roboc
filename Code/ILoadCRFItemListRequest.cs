using Svelto.ServiceLayer;

internal interface ILoadCRFItemListRequest : IServiceRequest<CRFShopItemListDependency>, IAnswerOnComplete<LoadCrfItemListRequestResponse>, IServiceRequest
{
}
