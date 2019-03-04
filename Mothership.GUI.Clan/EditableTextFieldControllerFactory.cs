using Mothership.GUI.Social;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	internal sealed class EditableTextFieldControllerFactory : IGUIElementFactory
	{
		public void Build(GameObject guiElementRoot, IContainer container)
		{
			EditableTextFieldView component = guiElementRoot.GetComponent<EditableTextFieldView>();
			EditableTextFieldController editableTextFieldController = container.Inject<EditableTextFieldController>(new EditableTextFieldController());
			component.InjectController(editableTextFieldController);
			editableTextFieldController.SetView(component);
		}
	}
}
