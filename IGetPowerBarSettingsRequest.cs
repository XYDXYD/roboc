using Svelto.ServiceLayer;

internal interface IGetPowerBarSettingsRequest : IServiceRequest, IAnswerOnComplete<PowerBarSettingsData>
{
	void ClearCache();

	void SetParameterOverride(ParameterOverride parameterOverride);
}
