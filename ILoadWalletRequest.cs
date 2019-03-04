using Svelto.ServiceLayer;

internal interface ILoadWalletRequest : IServiceRequest, IAnswerOnComplete<Wallet>
{
	void ClearCache();
}
