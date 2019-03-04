using Robocraft.GUI;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal class SearchClanLayoutFactory
	{
		private IContainer container;

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

		public SearchClanLayoutFactory(IContainer container)
		{
			this.container = container;
		}

		public void BuildAll(SearchClansView view, IDataSource clanListDataSource)
		{
			view.ClanListItemTemplate.set_name("SearchListItemTemplate");
			gameObjectPool.Preallocate(view.ClanListItemTemplate.get_name(), 200, (Func<GameObject>)(() => gameObjectPool.AddGameObjectWithInjectionInto<IGenericListEntryView>(view.ClanListItemTemplate, container)));
			BuildClanList(view, clanListDataSource);
			BuildSearchEntryWidget(view);
		}

		private void BuildSearchEntryWidget(SearchClansView view)
		{
			BuiltComponentElements builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TextEntry, null, view.SearchTextEntryTemplate);
			builtComponentElements.componentController.SetName("searchtextentry");
			builtComponentElements.componentView.AnchorThisElementUnder(view.SearchTextEntryContainerArea.GetComponent<UIWidget>());
			builtComponentElements.componentController.Activate();
		}

		private void BuildClanList(SearchClansView view, IDataSource clanListDataSource)
		{
			BuiltComponentElements builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.Button, null, view.LoadMoreButtonTemplate);
			(builtComponentElements.componentController as GenericButtonComponent).SetName("loadmore");
			(builtComponentElements.componentController as GenericButtonComponent).Activate();
			BuiltComponentElements builtComponentElements2 = GenericComponentFactory.BuildCustomComponent("ClanSearchList", clanListDataSource, view.ClanListTemplate);
			builtComponentElements2.componentController.SetName("clanlist");
			(builtComponentElements2.componentView as GenericListComponentView).SetListItemTemplate(view.ClanListItemTemplate.get_name(), view.ClanListItemTemplate);
			(builtComponentElements2.componentView as ClanSearchListComponentView).SetLoadMoreItemsButton(builtComponentElements.builtGameObject);
			UIRect component = view.ClanListContainerArea.GetComponent<UIWidget>();
			builtComponentElements2.componentView.AnchorThisElementUnder(component);
			(builtComponentElements.componentView as GenericButtonComponentView).ReparentOnly(builtComponentElements2.builtGameObject.get_transform());
			builtComponentElements2.componentController.Activate();
		}
	}
}
