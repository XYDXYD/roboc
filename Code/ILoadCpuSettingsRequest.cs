using Svelto.ServiceLayer;

internal interface ILoadCpuSettingsRequest : IServiceRequest, IAnswerOnComplete<CPUSettingsDependency>
{
}
