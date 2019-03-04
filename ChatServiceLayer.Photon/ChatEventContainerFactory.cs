using Services;
using Svelto.ServiceLayer;

namespace ChatServiceLayer.Photon
{
	internal class ChatEventContainerFactory : IChatEventContainerFactory, IEventContainerFactory
	{
		private readonly ChatEventRegistry _chatEventRegistry;

		public ChatEventContainerFactory(ChatEventRegistry chatEventRegistry)
		{
			_chatEventRegistry = chatEventRegistry;
		}

		public IServiceEventContainer Create()
		{
			return new ChatEventContainer(_chatEventRegistry);
		}
	}
}
