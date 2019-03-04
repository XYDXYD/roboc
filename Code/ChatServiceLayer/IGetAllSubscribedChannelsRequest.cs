using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace ChatServiceLayer
{
	internal interface IGetAllSubscribedChannelsRequest : IServiceRequest, IAnswerOnComplete<IEnumerable<ChatChannelInfo>>
	{
	}
}
