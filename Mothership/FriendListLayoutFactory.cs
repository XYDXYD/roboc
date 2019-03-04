using Mothership.GUI.Social;
using Robocraft.GUI;
using Svelto.IoC;
using System;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal class FriendListLayoutFactory : IGUIElementFactory
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

		public void BuildAll(FriendListView view, FriendListDataSource friendListDataSource, FriendListHeadersDataSource friendListHeadersDataSource, IContainer container)
		{
			view.FriendListItemTemplate.set_name("FriendListItemTemplate");
			gameObjectPool.Preallocate(view.FriendListItemTemplate.get_name(), 200, (Func<GameObject>)(() => gameObjectPool.AddGameObjectWithInjectionInto<IGenericListEntryView>(view.FriendListItemTemplate, container)));
			view.FriendListHeaderItemTemplate.set_name("FriendListItemTemplateHeader");
			gameObjectPool.Preallocate(view.FriendListHeaderItemTemplate.get_name(), 5, (Func<GameObject>)(() => gameObjectPool.AddGameObjectWithoutRecycle(view.FriendListHeaderItemTemplate)));
			BuildPlayerList(view, friendListDataSource, friendListHeadersDataSource);
		}

		public void Build(GameObject guiElementRoot, IContainer container)
		{
			Console.LogError("Build called in FriendLayoutFactory");
		}

		private void BuildPlayerList(FriendListView view, FriendListDataSource friendListDataSource, FriendListHeadersDataSource friendListHeaderDataSource)
		{
			BuiltComponentElements builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.Button, null, view.AddFriendButton);
			builtComponentElements.componentController.SetName("AddFriendButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.get_gameObject().get_transform());
			builtComponentElements = GenericComponentFactory.BuildCustomComponent(FriendComponentFactory.FriendList, friendListDataSource, view.FriendListTemplate);
			builtComponentElements.componentController.SetName("friendList");
			(builtComponentElements.componentView as GenericListComponentView).SetListItemTemplate(view.FriendListItemTemplate.get_name(), view.FriendListItemTemplate);
			(builtComponentElements.componentView as GenericExpandeableListComponentView).SetListItemHeader(view.FriendListHeaderItemTemplate, view.FriendListHeaderItemTemplate.get_name());
			(builtComponentElements.componentController as GenericExpandeableListComponent<FriendListEntryData>).SetHeadersDataSource(friendListHeaderDataSource);
			builtComponentElements.componentView.AnchorThisElementUnder(view.FriendListContainerArea.GetComponent<UIWidget>());
			builtComponentElements.componentController.Activate();
		}
	}
}
