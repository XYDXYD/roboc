using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer
{
	internal interface IDeclineFriendRequest : IServiceRequest<string>, IAnswerOnComplete<IList<Friend>>, IServiceRequest
	{
	}
}
