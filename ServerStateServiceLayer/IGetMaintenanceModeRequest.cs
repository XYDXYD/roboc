using Svelto.ServiceLayer;

namespace ServerStateServiceLayer
{
	internal interface IGetMaintenanceModeRequest : IServiceRequest, IAnswerOnComplete<MaintenanceModeData>
	{
	}
}
