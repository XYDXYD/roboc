using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IAcknowledgeRobotSanctionRequest : IServiceRequest<RobotSanctionData>, IServiceRequest
	{
	}
}
