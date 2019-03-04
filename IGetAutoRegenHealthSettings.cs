using Svelto.ServiceLayer;

internal interface IGetAutoRegenHealthSettings : IServiceRequest, IAnswerOnComplete<AutoRegenHealthSettingsData>
{
	void ClearCache();

	void SetParameterOverrides(ParameterOverride[] parameterOverrides);
}
