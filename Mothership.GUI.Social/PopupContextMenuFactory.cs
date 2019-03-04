using Mothership.GUI.Party;
using Robocraft.GUI;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI.Social
{
	internal sealed class PopupContextMenuFactory : IGUIElementFactory
	{
		[Inject]
		internal ClanPopupMenuController clanPopupMenuController
		{
			private get;
			set;
		}

		[Inject]
		internal FriendPopupMenuController friendPopupMenuController
		{
			private get;
			set;
		}

		[Inject]
		internal PartyPopupMenuController partyPopupMenuController
		{
			private get;
			set;
		}

		public void Build(GameObject guiElementRoot, IContainer container)
		{
			GenericPopupMenuView component = guiElementRoot.GetComponent<GenericPopupMenuView>();
			switch (component.popupMenuType)
			{
			case PopupMenuType.Clan:
				component.SetViewName("clanPopupMenuController");
				component.InjectController(clanPopupMenuController);
				clanPopupMenuController.SetView(component);
				break;
			case PopupMenuType.Friend:
				component.SetViewName("friendPopupMenuController");
				component.InjectController(friendPopupMenuController);
				friendPopupMenuController.SetView(component);
				break;
			case PopupMenuType.Party:
				component.SetViewName("partyPopupMenuController");
				component.InjectController(partyPopupMenuController);
				partyPopupMenuController.SetView(component);
				break;
			}
		}
	}
}
