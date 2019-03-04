using Svelto.Context;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership.GUI.Social
{
	internal sealed class SocialGUIFactory
	{
		[Inject]
		internal IContextNotifer contextNotifier
		{
			private get;
			set;
		}

		public void Build(GameObject socialGuiNode, IContainer container)
		{
			SocialGUIView component = socialGuiNode.GetComponent<SocialGUIView>();
			SocialGUIController socialGUIController = container.Inject<SocialGUIController>(new SocialGUIController());
			socialGUIController.RegisterToContextNotifier(contextNotifier);
			component.InjectController(socialGUIController);
			socialGUIController.SetView(component);
			IGUIFactoryType[] componentsInChildren = socialGuiNode.GetComponentsInChildren<IGUIFactoryType>();
			foreach (IGUIFactoryType iGUIFactoryType in componentsInChildren)
			{
				GameObject gameObject = (iGUIFactoryType as MonoBehaviour).get_gameObject();
				object obj = Activator.CreateInstance(iGUIFactoryType.guiElementFactoryType);
				container.Inject<object>(obj);
				(obj as IGUIElementFactory).Build(gameObject, container);
			}
		}
	}
}
