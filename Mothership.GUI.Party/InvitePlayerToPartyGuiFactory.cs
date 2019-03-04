using Robocraft.GUI;
using Robocraft.GUI.Iteration2;
using SocialServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;
using UnityEngine;

namespace Mothership.GUI.Party
{
	internal class InvitePlayerToPartyGuiFactory
	{
		public const string PLAYER_NAME_TEXT_FIELD = "PlayerNameTextField";

		public const string ERROR_LABEL = "PartyInviteGUIStringErrorLabel";

		public const string PARTY_INVITATION_BUTTON = "PartyInvitationButton";

		[Inject]
		internal IComponentFactory componentFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory webRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ISocialEventContainerFactory socialEventContainerFactory
		{
			private get;
			set;
		}

		public void Build(GameObject guiElementRoot, IContainer container)
		{
			guiElementRoot.SetActive(false);
			InvitePlayerToPartyGuiView component = guiElementRoot.GetComponent<InvitePlayerToPartyGuiView>();
			BuiltComponentElements builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Button, null, component.sendInviteButton, makeInstance: false);
			builtComponentElements.componentController.SetName("PartyInvitationButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(component.sendInviteButton.get_transform().get_parent());
			builtComponentElements = componentFactory.BuildComponent(GUIComponentType.TextEntry, null, component.playerNameTextField, makeInstance: false);
			builtComponentElements.componentController.SetName("PlayerNameTextField");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(component.playerNameTextField.get_transform().get_parent());
			InvitePlayerToPartyGuiController invitePlayerToPartyGuiController = container.Inject<InvitePlayerToPartyGuiController>(new InvitePlayerToPartyGuiController());
			component.SetController(invitePlayerToPartyGuiController);
			invitePlayerToPartyGuiController.SetView(component);
			OnlineFriendListDataSource onlineFriendListDataSource = new OnlineFriendListDataSource(socialRequestFactory, webRequestFactory);
			IServiceEventContainer socialEventContainer = socialEventContainerFactory.Create();
			OnlineClanMembersDataSource onlineClanMembersDataSource = new OnlineClanMembersDataSource(socialRequestFactory, webRequestFactory, socialEventContainer);
			InvitablePlayerItemFactory itemFactory = container.Inject<InvitablePlayerItemFactory>(new InvitablePlayerItemFactory(container));
			GenericExpandableListsFactory genericExpandableListsFactory = new GenericExpandableListsFactory();
			genericExpandableListsFactory.Add(onlineFriendListDataSource, itemFactory, component.playerListItemTemplate, "strFriends");
			genericExpandableListsFactory.Add(onlineClanMembersDataSource, itemFactory, component.playerListItemTemplate, "strClan");
			genericExpandableListsFactory.Build(component.playerListContainer, component.playerListHeaderTemplate);
			invitePlayerToPartyGuiController.SetDataSources(onlineFriendListDataSource, onlineClanMembersDataSource);
		}
	}
}
