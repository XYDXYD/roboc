using Svelto.ServiceLayer;

internal interface ICheckIfMachineExists : IServiceRequest, IAnswerOnComplete<bool>
{
}
