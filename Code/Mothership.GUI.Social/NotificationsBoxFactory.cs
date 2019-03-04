using Svelto.Context;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI.Social
{
	internal sealed class NotificationsBoxFactory : IGUIElementFactory
	{
		[Inject]
		internal IContextNotifer contextNotifier
		{
			private get;
			set;
		}

		public void Build(GameObject guiElementRoot, IContainer container)
		{
			NotificationsBoxView component = guiElementRoot.GetComponent<NotificationsBoxView>();
			switch (component.notificationsBoxType)
			{
			case NotificationsBoxType.Clan:
			{
				component.SetViewName("ClanNotificationsBoxRoot");
				NotificationsClanBoxController notificationsClanBoxController = container.Inject<NotificationsClanBoxController>(new NotificationsClanBoxController());
				contextNotifier.AddFrameworkDestructionListener(notificationsClanBoxController);
				component.InjectController(notificationsClanBoxController);
				notificationsClanBoxController.SetView(component);
				break;
			}
			case NotificationsBoxType.Friend:
			{
				component.SetViewName("FriendNotificationsBoxRoot");
				NotificationsFriendBoxController notificationsFriendBoxController = container.Inject<NotificationsFriendBoxController>(new NotificationsFriendBoxController());
				contextNotifier.AddFrameworkDestructionListener(notificationsFriendBoxController);
				component.InjectController(notificationsFriendBoxController);
				notificationsFriendBoxController.SetView(component);
				break;
			}
			}
		}
	}
}
