using SocialServiceLayer.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace SocialServiceLayer
{
	internal interface IPollClanExperienceRequest : IServiceRequest<string>, IAnswerOnComplete<PollClanExperienceRequestResponse>, ITask, IServiceRequest, IAbstractTask
	{
	}
}
