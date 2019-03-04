using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface ICheckRobotSanctionRequest : IServiceRequest<string>, IAnswerOnComplete<RobotSanctionData>, IServiceRequest
	{
	}
}
