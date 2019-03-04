using Svelto.ServiceLayer;
using Svelto.Tasks;

internal interface ISetGarageSlotControlsRequest : IServiceRequest<GarageSlotControlsDependency>, IAnswerOnComplete<ControlSettings>, ITask, IServiceRequest, IAbstractTask
{
}
