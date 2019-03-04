using Robocraft.GUI;
using UnityEngine;

namespace Mothership.GUI.Social
{
	internal abstract class NotificationsBoxControllerBase
	{
		protected NotificationsBoxView _view;

		public virtual void PlayAlertSound(GameObject source)
		{
		}

		public void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
				if (genericComponentMessage.Target == _view.Name || (string.IsNullOrEmpty(genericComponentMessage.Target) && genericComponentMessage.Originator != _view.Name))
				{
					HandleGenericMessage(genericComponentMessage);
				}
			}
			else
			{
				HandleSignalChainMessage(message);
			}
		}

		public virtual void HandleGenericMessage(GenericComponentMessage genericMessage)
		{
		}

		public virtual void HandleSignalChainMessage(object Message)
		{
		}

		public virtual void SetView(NotificationsBoxView view)
		{
			_view = view;
		}

		protected void DispatchGenericMessage(GenericComponentMessage message)
		{
			_view.BroadcastGenericMessage(message);
		}
	}
}
