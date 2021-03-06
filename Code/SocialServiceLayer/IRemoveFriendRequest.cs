using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer
{
	internal interface IRemoveFriendRequest : IServiceRequest<string>, IAnswerOnComplete<IList<Friend>>, IServiceRequest
	{
	}
}
