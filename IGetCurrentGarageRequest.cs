using Services.Mothership;
using Svelto.ServiceLayer;

internal interface IGetCurrentGarageRequest : IServiceRequest, IAnswerOnComplete<GarageSlotData>
{
}
