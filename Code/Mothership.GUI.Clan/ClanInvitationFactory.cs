using Mothership.GUI.Social;
using Robocraft.GUI;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	internal sealed class ClanInvitationFactory : IGUIElementFactory
	{
		[Inject]
		internal IComponentFactory componentFactory
		{
			private get;
			set;
		}

		public void Build(GameObject guiElementRoot, IContainer container)
		{
			ClanInvitationView component = guiElementRoot.GetComponent<ClanInvitationView>();
			ClanInvitationController clanInvitationController = container.Inject<ClanInvitationController>(new ClanInvitationController());
			component.InjectController(clanInvitationController);
			clanInvitationController.SetView(component);
			BuiltComponentElements builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Button, null, component.inviteClanButton);
			builtComponentElements.componentController.SetName("InviteClanButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(component.inviteClanButton.get_transform().get_parent());
			builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Button, null, component.closeClanInvitationButton);
			builtComponentElements.componentController.SetName("CloseClanInvitationButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(component.closeClanInvitationButton.get_transform().get_parent());
			builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Button, null, component.dismissSuccesfulPopupButton);
			builtComponentElements.componentController.SetName("DismissSuccesfulPopupButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(component.dismissSuccesfulPopupButton.get_transform().get_parent());
			builtComponentElements = componentFactory.BuildComponent(GUIComponentType.TextEntry, null, component.playerNameTextField);
			builtComponentElements.componentController.SetName("PlayerNameTextField");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(component.playerNameTextField.get_transform().get_parent());
			builtComponentElements = componentFactory.BuildComponent(GUIComponentType.TextLabel, null, component.errorLabel);
			builtComponentElements.componentController.SetName("InviteSomeoneErrorLabel");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(guiElementRoot.get_transform());
		}
	}
}
