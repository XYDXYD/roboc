using Svelto.ServiceLayer;

internal interface ILoadPremiumDataRequest : IServiceRequest, IAnswerOnComplete<PremiumInfoData>
{
	void ClearCache();
}
