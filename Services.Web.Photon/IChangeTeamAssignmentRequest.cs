using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IChangeTeamAssignmentRequest : IServiceRequest<ChangeCustomGameTeamAssignmentDependancy>, IAnswerOnComplete<ChangeCustomGameTeamAssignmentResponse>, IServiceRequest
	{
	}
}
