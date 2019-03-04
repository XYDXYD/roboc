using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer
{
	internal interface ICancelFriendRequest : IServiceRequest<string>, IAnswerOnComplete<IList<Friend>>, IServiceRequest
	{
	}
}
