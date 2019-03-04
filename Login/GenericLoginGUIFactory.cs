using Mothership.GUI.Social;
using Robocraft.GUI;
using Svelto.Context;
using Svelto.IoC;
using UnityEngine;

namespace Login
{
	internal sealed class GenericLoginGUIFactory : IGUIElementFactory
	{
		[Inject]
		internal IContextNotifer contextNotifier
		{
			private get;
			set;
		}

		[Inject]
		internal IComponentFactory componentFactory
		{
			private get;
			set;
		}

		public void Build(GameObject guiElementRoot, IContainer container)
		{
			GenericLoginView component = guiElementRoot.GetComponent<GenericLoginView>();
			GenericLoginController genericLoginController = container.Inject<GenericLoginController>(new GenericLoginController());
			component.InjectController(genericLoginController);
			genericLoginController.SetView(component);
			genericLoginController.Initialise();
			contextNotifier.AddFrameworkInitializationListener(genericLoginController);
			contextNotifier.AddFrameworkDestructionListener(genericLoginController);
		}
	}
}
