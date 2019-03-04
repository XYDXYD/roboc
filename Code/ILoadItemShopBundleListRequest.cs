using Svelto.ServiceLayer;
using Svelto.Tasks;

internal interface ILoadItemShopBundleListRequest : IServiceRequest, IAnswerOnComplete<ItemShopResponseData>, ITask, IAbstractTask
{
	void ClearCache();
}
