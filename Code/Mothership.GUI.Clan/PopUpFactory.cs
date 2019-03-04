using Mothership.GUI.Social;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	internal sealed class PopUpFactory : IGUIElementFactory
	{
		public void Build(GameObject guiElementRoot, IContainer container)
		{
			PopUpView component = guiElementRoot.GetComponent<PopUpView>();
			PopUpController popUpController = container.Inject<PopUpController>(new PopUpController());
			component.InjectController(popUpController);
			popUpController.SetView(component);
		}
	}
}
