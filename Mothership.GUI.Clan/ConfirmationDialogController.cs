using Robocraft.GUI;

namespace Mothership.GUI.Clan
{
	internal sealed class ConfirmationDialogController
	{
		private ConfirmationDialogView _view;

		public void HandleGenericMessage(GenericComponentMessage message)
		{
			if (message.Message == MessageType.Show)
			{
				_view.get_gameObject().SetActive(true);
			}
			else if (message.Message == MessageType.Hide)
			{
				_view.get_gameObject().SetActive(false);
			}
		}

		public void SetView(ConfirmationDialogView view)
		{
			_view = view;
		}
	}
}
