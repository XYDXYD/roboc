using Svelto.ServiceLayer;

internal interface ILoadCRFShopItemDataRequest : IServiceRequest<int>, IAnswerOnComplete<LoadCRFShopItemDataRequestResponse>, IServiceRequest
{
	void ClearTheCache();
}
