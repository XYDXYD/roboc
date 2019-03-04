using Robocraft.GUI;
using Svelto.Context;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using UnityEngine;

namespace Mothership.GUI.CustomGames
{
	internal sealed class CustomGamePartyGUIFactory
	{
		[Inject]
		internal IContextNotifer contextNotifier
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGamePopupMenuController popupMenuController
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGamePartyGUIController customGamePartyController
		{
			private get;
			set;
		}

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		public void Build(GameObject rootNode, IContainer container)
		{
			CustomGamePartyGUIView component = rootNode.GetComponent<CustomGamePartyGUIView>();
			customGamePartyController.RegisterToContextNotifier(contextNotifier);
			component.InjectController(customGamePartyController);
			customGamePartyController.SetView(component);
			component.Initialize();
			CustomGameTeamView[] componentsInChildren = rootNode.GetComponentsInChildren<CustomGameTeamView>();
			CustomGameTeamController[] array = new CustomGameTeamController[2];
			gameObjectPool.Preallocate("CustomGamePartyIconPool", 20, (Func<GameObject>)CreateCustomGamePartyIcon);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				array[i] = new CustomGameTeamController();
				componentsInChildren[i].InjectController(array[i]);
				componentsInChildren[i].Initialise();
				container.Inject<CustomGameTeamController>(array[i]);
				array[i].SetView(componentsInChildren[i]);
			}
			customGamePartyController.SetTeamControllers(array[0], array[1]);
			GenericPopupMenuView componentInChildren = rootNode.GetComponentInChildren<GenericPopupMenuView>(true);
			componentInChildren.SetViewName("customgamePopupMenuController");
			componentInChildren.InjectController(popupMenuController);
			popupMenuController.SetView(componentInChildren);
		}

		private GameObject CreateCustomGamePartyIcon()
		{
			GameObject val = gameObjectFactory.Build("CustomGamePartyButton");
			val.SetActive(false);
			return val;
		}
	}
}
