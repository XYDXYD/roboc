using Svelto.ServiceLayer;

internal interface ISaveMachineColorRequest : IServiceRequest<SaveMachineColorDependency>, IAnswerOnComplete, IServiceRequest
{
}
