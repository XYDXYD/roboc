using Robocraft.GUI;

namespace Mothership
{
	public interface IClanSectionController : IGenericMessageDispatcher
	{
		void SetView(ClanSectionViewBase tabViewBase);

		void HandleClanMessage(SocialMessage receivedMessage);

		void HandleGenericMessage(GenericComponentMessage receivedMessage);

		void HandleMessage(object receivedMessage);

		void Show();

		void Hide();
	}
}
