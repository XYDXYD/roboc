using Mothership.GUI.Social;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	internal sealed class ConfirmationDialogFactory : IGUIElementFactory
	{
		public void Build(GameObject guiElementRoot, IContainer container)
		{
			ConfirmationDialogView component = guiElementRoot.GetComponent<ConfirmationDialogView>();
			ConfirmationDialogController confirmationDialogController = container.Inject<ConfirmationDialogController>(new ConfirmationDialogController());
			component.viewName = "LeaveConfirmationDialog";
			component.InjectController(confirmationDialogController);
			confirmationDialogController.SetView(component);
		}
	}
}
