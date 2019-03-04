using Svelto.ServiceLayer;

internal interface ISaveMachineRequest : IServiceRequest<SaveMachineDependency>, IAnswerOnComplete<SaveMachineResult>, IServiceRequest
{
}
