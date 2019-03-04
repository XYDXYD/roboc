using Mothership.GUI.Social;
using Robocraft.GUI;
using Svelto.Context;
using Svelto.IoC;
using UnityEngine;

namespace Login
{
	internal sealed class FadeInOutImageFactory : IGUIElementFactory
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
			FadeInOutImageView component = guiElementRoot.GetComponent<FadeInOutImageView>();
			FadeInOutImageController fadeInOutImageController = container.Inject<FadeInOutImageController>(new FadeInOutImageController());
			component.InjectController(fadeInOutImageController);
			fadeInOutImageController.SetView(component);
			fadeInOutImageController.Initialise();
			component.Initialise();
		}
	}
}
