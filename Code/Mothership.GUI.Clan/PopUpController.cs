using Robocraft.GUI;

namespace Mothership.GUI.Clan
{
	internal sealed class PopUpController
	{
		private PopUpView _view;

		public void HandleMessage(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
				if (genericComponentMessage.Message == MessageType.Show && genericComponentMessage.Target == _view.popUpName)
				{
					_view.get_gameObject().SetActive(true);
				}
			}
		}

		public void SetView(PopUpView view)
		{
			_view = view;
		}
	}
}
