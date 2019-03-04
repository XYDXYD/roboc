using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace ChatServiceLayer
{
	internal interface ISubscribeAllJoinedChannelsRequest : IServiceRequest, IAnswerOnComplete<IEnumerable<ChatChannelInfo>>
	{
	}
}
