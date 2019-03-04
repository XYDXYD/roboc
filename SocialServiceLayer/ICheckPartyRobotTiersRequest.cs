using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface ICheckPartyRobotTiersRequest : IServiceRequest, IAnswerOnComplete<bool>
	{
	}
}
