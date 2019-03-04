using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer
{
	internal interface IFriendStatusEventListener : IServiceEventListener<Friend, IList<Friend>>, IServiceEventListenerBase
	{
	}
}
