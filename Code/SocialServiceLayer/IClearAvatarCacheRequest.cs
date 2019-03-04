using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace SocialServiceLayer
{
	internal interface IClearAvatarCacheRequest : IServiceRequest, ITask, IAbstractTask
	{
		void Inject(List<string> clearOnlyThesePlayers, List<string> clearOnlyTheseClans);
	}
}
