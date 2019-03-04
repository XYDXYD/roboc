using Svelto.ServiceLayer;

internal interface ILoadMachineRequest : IServiceRequest<uint?>, IAnswerOnComplete<LoadMachineResult>, IServiceRequest
{
	void ForceClearCache();
}
