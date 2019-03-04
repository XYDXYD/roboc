using Robocraft.GUI;
using Svelto.IoC;

namespace Mothership
{
	public interface IFriendSectionController : IGenericMessageDispatcher
	{
		void SetView(FriendSectionViewBase tabViewBase);

		void BuildLayout(IContainer container);

		void HandleFriendMessage(SocialMessage receivedMessage);

		void HandleGenericMessage(GenericComponentMessage receivedMessage);

		void HandleMessage(object message);

		void Show();

		void Hide();
	}
}
