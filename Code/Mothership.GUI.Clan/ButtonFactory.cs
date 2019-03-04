using Mothership.GUI.Social;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	internal sealed class ButtonFactory : IGUIElementFactory
	{
		public void Build(GameObject guiElementRoot, IContainer container)
		{
			ButtonView component = guiElementRoot.GetComponent<ButtonView>();
			ButtonController buttonController = new ButtonController();
			component.InjectController(buttonController);
			buttonController.SetView(component);
		}
	}
}
