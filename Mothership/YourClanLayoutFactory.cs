using Robocraft.GUI;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal class YourClanLayoutFactory
	{
		private IContainer container;

		[Inject]
		internal IComponentFactory GenericComponentFactory
		{
			get;
			private set;
		}

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		public YourClanLayoutFactory(IContainer container)
		{
			this.container = container;
		}

		public void BuildAll(YourClanView view, IDataSource yourClanInfoDataSource, IDataSource yourClanAvatarImageDataSource, IDataSource playersDataSource, IDataSource playerHeadersDataSource)
		{
			view.PlayerListItemTemplate.set_name("YourClanPlayerListTemplate");
			gameObjectPool.Preallocate(view.PlayerListItemTemplate.get_name(), 50, (Func<GameObject>)(() => gameObjectPool.AddGameObjectWithInjectionInto<IGenericListEntryView>(view.PlayerListItemTemplate, container)));
			view.PlayerListHeaderItemTemplate.set_name("YourClanPlayerListHeaderTemplate");
			gameObjectPool.Preallocate(view.PlayerListHeaderItemTemplate.get_name(), 5, (Func<GameObject>)(() => gameObjectPool.AddGameObjectWithoutRecycle(view.PlayerListHeaderItemTemplate)));
			BuildPlayerList(view, playersDataSource, playerHeadersDataSource);
			BuildTextLabels(view, yourClanInfoDataSource, yourClanAvatarImageDataSource);
		}

		private void BuildTextLabels(YourClanView view, IDataSource yourClanInfoDataSource, IDataSource yourClanAvatarImageDataSource)
		{
			BuiltComponentElements builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TextLabel, yourClanInfoDataSource, view.yourclanDescriptionTextLabelTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("clandescription");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.yourclanDescriptionTextLabelTemplate.get_transform().get_parent());
			(builtComponentElements.componentController as GenericTextLabelComponent).SetDataSourceIndex(1);
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TextEntry, yourClanInfoDataSource, view.yourclanDescriptionTextEntryTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("ClanDescriptionTextEntry");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.yourclanDescriptionTextEntryTemplate.get_transform().get_parent());
			(builtComponentElements.componentController as GenericTextEntryComponent).SetDataSourceIndex(1);
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TextLabel, yourClanInfoDataSource, view.yourclanNameTextLabelTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("clanname");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.yourclanNameTextLabelTemplate.get_transform().get_parent());
			(builtComponentElements.componentController as GenericTextLabelComponent).SetDataSourceIndex(0);
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TextLabel, yourClanInfoDataSource, view.yourclanInviteStatusTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("claninvitestatus");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.yourclanInviteStatusTemplate.get_transform().get_parent());
			(builtComponentElements.componentController as GenericTextLabelComponent).SetDataSourceIndex(2);
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.PopUpList, yourClanInfoDataSource, view.yourclanInviteStatusPopUpListTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("ClanTypePopUpList");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.yourclanInviteStatusPopUpListTemplate.get_transform().get_parent());
			(builtComponentElements.componentController as GenericPopUpListComponent).SetDataSourceIndices(2, 0);
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.Image, yourClanAvatarImageDataSource, view.yourclanAvatarImageTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("avatarImage");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.yourclanAvatarImageTemplate.get_transform().get_parent());
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.Button, null, view.yourclanUploadAvatarImageButtonTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("UploadAvatarImageButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.yourclanUploadAvatarImageButtonTemplate.get_transform().get_parent());
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.Button, null, view.LeaveButtonTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("LeaveButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.LeaveButtonTemplate.get_transform().get_parent());
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.Button, null, view.JoinButtonTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("JoinButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.JoinButtonTemplate.get_transform().get_parent());
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.Button, null, view.InviteButtonTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("InviteButton");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.InviteButtonTemplate.get_transform().get_parent());
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TextLabel, null, view.ErrorLabelTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("yourClanErrorLabel");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.ErrorLabelTemplate.get_transform().get_parent());
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TextLabel, yourClanInfoDataSource, view.seasonExperienceLabelTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("seasonExperienceLabel");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.seasonExperienceLabelTemplate.get_transform().get_parent());
			(builtComponentElements.componentController as GenericTextLabelComponent).SetDataSourceIndex(3);
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TextLabel, yourClanInfoDataSource, view.seasonRobitsLabelTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("seasonRobitsLabel");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.seasonRobitsLabelTemplate.get_transform().get_parent());
			(builtComponentElements.componentController as GenericTextLabelComponent).SetDataSourceIndex(4);
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TextLabel, yourClanInfoDataSource, view.clanSizeLabelTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("clanSizeLabel");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.clanSizeLabelTemplate.get_transform().get_parent());
			(builtComponentElements.componentController as GenericTextLabelComponent).SetDataSourceIndex(5);
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TextLabel, yourClanInfoDataSource, view.yourclanDescriptionTooltipTextLabelTemplate, makeInstance: false);
			builtComponentElements.componentController.SetName("clandescriptionTooltip");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.yourclanDescriptionTooltipTextLabelTemplate.get_transform().get_parent());
			(builtComponentElements.componentController as GenericTextLabelComponent).SetDataSourceIndex(1);
		}

		private void BuildPlayerList(YourClanView view, IDataSource playerListDataSource, IDataSource playerListHeadersDataSource)
		{
			BuiltComponentElements builtComponentElements = GenericComponentFactory.BuildCustomComponent("ClanPlayerList", playerListDataSource, view.PlayerListTemplate);
			builtComponentElements.componentController.SetName("playerlist");
			(builtComponentElements.componentView as GenericListComponentView).SetListItemTemplate(view.PlayerListItemTemplate.get_name(), view.PlayerListItemTemplate);
			(builtComponentElements.componentView as GenericExpandeableListComponentView).SetListItemHeader(view.PlayerListHeaderItemTemplate, view.PlayerListHeaderItemTemplate.get_name());
			(builtComponentElements.componentController as GenericExpandeableListComponent<ClanMemberWithContextInfo>).SetHeadersDataSource(playerListHeadersDataSource);
			(builtComponentElements.componentView as ClanPlayersListComponentView).SetWarningMessageGO(view.LessThanTenMembersWarningMessage);
			builtComponentElements.componentView.AnchorThisElementUnder(view.PlayerListContainerArea.GetComponent<UIWidget>());
			builtComponentElements.componentController.Activate();
		}
	}
}
