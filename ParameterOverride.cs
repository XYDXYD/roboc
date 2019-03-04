using Services.Web.Photon;

public struct ParameterOverride
{
	public readonly WebServicesParameterCode ParameterCode;

	public readonly object ParameterValue;

	public ParameterOverride(WebServicesParameterCode parameterCode, object parameterValue)
	{
		ParameterCode = parameterCode;
		ParameterValue = parameterValue;
	}
}
