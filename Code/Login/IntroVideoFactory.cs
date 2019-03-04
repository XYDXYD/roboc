using Mothership.GUI.Social;
using Robocraft.GUI;
using Svelto.Context;
using Svelto.IoC;
using UnityEngine;

namespace Login
{
	internal sealed class IntroVideoFactory : IGUIElementFactory
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
			IntroVideoView component = guiElementRoot.GetComponent<IntroVideoView>();
			IntroVideoController introVideoController = container.Inject<IntroVideoController>(new IntroVideoController());
			component.InjectController(introVideoController);
			introVideoController.SetView(component);
			introVideoController.Initialise();
			component.Initialise();
		}
	}
}
