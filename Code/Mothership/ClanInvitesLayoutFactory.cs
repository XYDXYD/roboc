using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal class ClanInvitesLayoutFactory
	{
		[Inject]
		internal IComponentFactory GenericComponentFactory
		{
			get;
			private set;
		}

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		public void BuildAll(ClanInvitesView view, IDataSource inviteesListDataSource, IDataSource inviteesListHeadersDataSource, IContainer container)
		{
			view.InviteesListItemTemplate.set_name("ClanInvitesItemTemplate");
			gameObjectPool.Preallocate(view.InviteesListItemTemplate.get_name(), 50, (Func<GameObject>)(() => gameObjectPool.AddGameObjectWithInjectionInto<IGenericListEntryView>(view.InviteesListItemTemplate, container)));
			view.InviteesListHeaderItemTemplate.set_name("ClanInvitesItemTemplateHeader");
			gameObjectPool.Preallocate(view.InviteesListHeaderItemTemplate.get_name(), 5, (Func<GameObject>)(() => gameObjectPool.AddGameObjectWithoutRecycle(view.InviteesListHeaderItemTemplate)));
			BuildInviteesList(view, inviteesListDataSource, inviteesListHeadersDataSource);
			BuildButtons(view);
		}

		private void BuildButtons(ClanInvitesView view)
		{
			BuiltComponentElements builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.Button, null, view.DeclineAllButtonTemplate);
			builtComponentElements.componentController.SetName("declineall");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.get_gameObject().get_transform());
		}

		private void BuildInviteesList(ClanInvitesView view, IDataSource inviteesListDataSource, IDataSource inviteesListHeadersDataSource)
		{
			BuiltComponentElements builtComponentElements = GenericComponentFactory.BuildCustomComponent("ClanInvitationsList", inviteesListDataSource, view.InviteesListTemplate);
			builtComponentElements.componentController.SetName("inviteeslist");
			(builtComponentElements.componentView as GenericExpandeableListComponentView).SetListItemTemplate(view.InviteesListItemTemplate.get_name(), view.InviteesListItemTemplate);
			(builtComponentElements.componentView as GenericExpandeableListComponentView).SetListItemHeader(view.InviteesListHeaderItemTemplate, view.InviteesListHeaderItemTemplate.get_name());
			(builtComponentElements.componentController as GenericExpandeableListComponent<ClanInvite>).SetHeadersDataSource(inviteesListHeadersDataSource);
			builtComponentElements.componentView.AnchorThisElementUnder(view.InviteesListContainerArea.GetComponent<UIWidget>());
			builtComponentElements.componentController.Activate();
		}
	}
}
