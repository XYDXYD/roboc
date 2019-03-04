using Svelto.ServiceLayer;

internal interface ILoadMachineColorMapRequest : IServiceRequest<LoadMachineColorMapDependancy>, IAnswerOnComplete<byte[]>, IServiceRequest
{
}
