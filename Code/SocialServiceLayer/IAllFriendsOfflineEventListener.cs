using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer
{
	internal interface IAllFriendsOfflineEventListener : IServiceEventListener<IList<Friend>>, IServiceEventListenerBase
	{
	}
}
