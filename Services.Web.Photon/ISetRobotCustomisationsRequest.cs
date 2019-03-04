using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace Services.Web.Photon
{
	internal interface ISetRobotCustomisationsRequest : IServiceRequest<SetRobotCustomisationDependency>, IAnswerOnComplete<SetRobotCustomisationDependency>, ITask, IServiceRequest, IAbstractTask
	{
	}
}
