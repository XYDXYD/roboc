using Mothership.GUI.Clan;
using Mothership.GUI.Social;
using Svelto.Context;
using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal class ClanGeneralGUIFactory : IGUIElementFactory
	{
		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IContextNotifer contextNotifier
		{
			private get;
			set;
		}

		public void Build(GameObject guiElementRoot, IContainer container)
		{
			IClanSectionController clanSectionController = container.Inject<CreateClanController>(new CreateClanController());
			IClanSectionController clanSectionController2 = container.Inject<YourClanController>(new YourClanController());
			IClanSectionController clanSectionController3 = container.Inject<SearchClansController>(new SearchClansController());
			ClanInvitesController clanInvitesController = container.Inject<ClanInvitesController>(new ClanInvitesController());
			YourClanEditingController yourClanEditingController = container.Inject<YourClanEditingController>(new YourClanEditingController());
			contextNotifier.AddFrameworkDestructionListener(clanSectionController2 as YourClanController);
			ClanSectionViewBase component = gameObjectFactory.Build("CreateClanScreen").GetComponent<ClanSectionViewBase>();
			GameObject val = gameObjectFactory.Build("YourClanContainerMK2_v2");
			ClanSectionViewBase component2 = val.GetComponent<ClanSectionViewBase>();
			YourClanEditingView component3 = val.GetComponent<YourClanEditingView>();
			ClanSectionViewBase component4 = gameObjectFactory.Build("SearchClansContainer2").GetComponent<ClanSectionViewBase>();
			ClanSectionViewBase component5 = gameObjectFactory.Build("ClanInvitationsContainer").GetComponent<ClanSectionViewBase>();
			component.Hide();
			component2.Hide();
			component4.Hide();
			component5.Hide();
			ClanScreen component6 = guiElementRoot.GetComponent<ClanScreen>();
			Transform transform = component6.ClanMainContainer.get_transform();
			component.get_transform().set_parent(transform);
			component2.get_transform().set_parent(transform);
			component4.get_transform().set_parent(transform);
			component5.get_transform().set_parent(transform);
			IGUIFactoryType[] componentsInChildren = transform.GetComponentsInChildren<IGUIFactoryType>(true);
			foreach (IGUIFactoryType iGUIFactoryType in componentsInChildren)
			{
				GameObject gameObject = (iGUIFactoryType as MonoBehaviour).get_gameObject();
				object obj = Activator.CreateInstance(iGUIFactoryType.guiElementFactoryType);
				container.Inject<object>(obj);
				(obj as IGUIElementFactory).Build(gameObject, container);
			}
			clanSectionController.SetView(component);
			yourClanEditingController.SetView(component3);
			component3.InjectController(yourClanEditingController);
			clanSectionController2.SetView(component2);
			clanSectionController3.SetView(component4);
			clanInvitesController.SetView(component5);
			clanInvitesController.BuildLayout(container);
		}
	}
}
