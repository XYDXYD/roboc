using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer
{
	internal interface IAcceptFriendRequest : IServiceRequest<string>, IAnswerOnComplete<IList<Friend>>, IServiceRequest
	{
	}
}
