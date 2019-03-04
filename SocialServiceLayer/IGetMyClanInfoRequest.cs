using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IGetMyClanInfoRequest : IServiceRequest, IAnswerOnComplete<ClanInfo>
	{
	}
}
