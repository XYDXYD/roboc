using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface ICheckAllRobotsSanctionsRequest : IServiceRequest, IAnswerOnComplete<RobotSanctionData[]>
	{
	}
}
