using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IGetFriendListRequest : IServiceRequest, IAnswerOnComplete<GetFriendListResponse>
	{
		bool ForceRefresh
		{
			get;
			set;
		}
	}
}
