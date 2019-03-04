using Mothership.GUI.Social;
using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal class FriendGeneralGUIFactory : IGUIElementFactory
	{
		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		public void Build(GameObject guiElementRoot, IContainer container)
		{
			IFriendSectionController friendSectionController = container.Inject<FriendListController>(new FriendListController());
			FriendSectionViewBase component = gameObjectFactory.Build("FriendListContainer").GetComponent<FriendSectionViewBase>();
			Transform transform = guiElementRoot.GetComponent<FriendScreen>().FriendMainContainer.get_transform();
			component.get_transform().set_parent(transform);
			IGUIFactoryType[] componentsInChildren = transform.GetComponentsInChildren<IGUIFactoryType>(true);
			foreach (IGUIFactoryType iGUIFactoryType in componentsInChildren)
			{
				GameObject gameObject = (iGUIFactoryType as MonoBehaviour).get_gameObject();
				object obj = Activator.CreateInstance(iGUIFactoryType.guiElementFactoryType);
				container.Inject<object>(obj);
				(obj as IGUIElementFactory).Build(gameObject, container);
			}
			friendSectionController.SetView(component);
			friendSectionController.BuildLayout(container);
		}
	}
}
