using Mothership.GUI.Social;
using Robocraft.GUI;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class AddFriendFactory : IGUIElementFactory
	{
		[Inject]
		internal IComponentFactory componentFactory
		{
			private get;
			set;
		}

		public void Build(GameObject guiElementRoot, IContainer container)
		{
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			AddFriendView component = guiElementRoot.GetComponent<AddFriendView>();
			AddFriendController addFriendController = container.Inject<AddFriendController>(new AddFriendController());
			component.InjectController(addFriendController);
			addFriendController.SetView(component);
			BuiltComponentElements builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Button, null, component.inviteFriendButton);
			builtComponentElements.componentController.SetName("FriendInvitationButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(component.inviteFriendButton.get_transform().get_parent());
			builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Button, null, component.closeFriendInvitationButton);
			builtComponentElements.componentController.SetName("CloseFriendInvitationButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(component.closeFriendInvitationButton.get_transform().get_parent());
			builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Button, null, component.dismissButton);
			builtComponentElements.componentController.SetName("DismissSuccessfulPopupButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(component.dismissButton.get_transform().get_parent());
			builtComponentElements = componentFactory.BuildComponent(GUIComponentType.TextEntry, null, component.playerNameTextField);
			builtComponentElements.componentController.SetName("PlayerNameTextField");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(component.playerNameTextField.get_transform().get_parent());
			builtComponentElements.builtGameObject.get_transform().set_position(component.playerNameTextField.get_transform().get_position());
			builtComponentElements = componentFactory.BuildComponent(GUIComponentType.TextLabel, null, component.errorLabel);
			builtComponentElements.componentController.SetName("FriendGUIStringErrorLabel");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(guiElementRoot.get_transform());
			builtComponentElements.builtGameObject.SetActive(false);
		}
	}
}
