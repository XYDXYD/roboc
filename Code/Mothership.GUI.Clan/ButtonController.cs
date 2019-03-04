using Robocraft.GUI;

namespace Mothership.GUI.Clan
{
	internal sealed class ButtonController
	{
		private ButtonView _view;

		public void SetView(ButtonView view)
		{
			_view = view;
		}

		public void HandleMessage(GenericComponentMessage message)
		{
			if (message.Message == MessageType.Disable && message.Target == _view.get_name())
			{
				_view.SetButtonActive(active: false);
			}
			else if (message.Message == MessageType.Enable && message.Target == _view.get_name())
			{
				_view.SetButtonActive(active: true);
			}
		}
	}
}
