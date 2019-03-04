using Robocraft.GUI;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership.GUI.Social
{
	internal class NotificationsBoxView : MonoBehaviour, IChainListener, IChainRoot, IGUIFactoryType
	{
		[SerializeField]
		private NotificationsBoxType notificationsBox;

		[SerializeField]
		private GameObject numberLabel;

		[SerializeField]
		private GameObject numberContainer;

		[SerializeField]
		private UISprite AlertSprite;

		private NotificationsBoxControllerBase _controller;

		private string _viewName;

		private SignalChain _signal;

		public NotificationsBoxType notificationsBoxType => notificationsBox;

		public GameObject numberLabelTemplate => numberLabel;

		public Type guiElementFactoryType => typeof(NotificationsBoxFactory);

		public string Name => _viewName;

		public NotificationsBoxView()
			: this()
		{
		}

		public void ShowAlert(bool show)
		{
			if (!AlertSprite.get_isActiveAndEnabled() && show)
			{
				_controller.PlayAlertSound(AlertSprite.get_gameObject());
			}
			AlertSprite.get_gameObject().SetActive(show);
		}

		public void ShowLabel(bool visible)
		{
			numberContainer.SetActive(visible);
		}

		public void Listen(object message)
		{
			_controller.Listen(message);
		}

		public void InjectController(NotificationsBoxControllerBase controller)
		{
			_controller = controller;
		}

		public void SetViewName(string viewName_)
		{
			_viewName = viewName_;
		}

		public void BroadcastGenericMessage(GenericComponentMessage message)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if (_signal == null)
			{
				_signal = new SignalChain(this.get_transform());
			}
			_signal.DeepBroadcast(typeof(GenericComponentMessage), (object)message);
		}
	}
}
