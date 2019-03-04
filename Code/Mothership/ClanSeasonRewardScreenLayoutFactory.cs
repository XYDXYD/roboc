using Robocraft.GUI;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class ClanSeasonRewardScreenLayoutFactory
	{
		[Inject]
		internal IComponentFactory GenericComponentFactory
		{
			get;
			private set;
		}

		public void BuildAll(ClanSeasonRewardScreenView view, ClanSeasonRewardDataSource dataSource)
		{
			BuiltComponentElements builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.Button, null, view.confirmButton, makeInstance: false);
			builtComponentElements.componentController.SetName(ClanSeasonRewardScreenComponentNames.CONFIRM_BUTTON_NAME);
			builtComponentElements.componentController.Activate();
			builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.Image, dataSource, view.clanAvatar, makeInstance: false);
			builtComponentElements.componentController.SetName(ClanSeasonRewardScreenComponentNames.CLAN_AVATAR_NAME);
			builtComponentElements.componentController.Activate();
			(builtComponentElements.componentController as GenericImageComponent).SetDataSourceIndex(2);
			BuildLabel(view.clanSeasonLabel, dataSource, 0, ClanSeasonRewardScreenComponentNames.CLAN_SEASON_LABEL_NAME);
			BuildLabel(view.robitsLabel, dataSource, 3, ClanSeasonRewardScreenComponentNames.ROBITS_REWARD_LABEL_NAME);
			BuildLabel(view.personalSeasonExperienceLabel, dataSource, 4, ClanSeasonRewardScreenComponentNames.PERSONAL_SEASON_EXPERIENCE_LABEL);
			BuildLabel(view.averageSeasonExperienceLabel, dataSource, 5, ClanSeasonRewardScreenComponentNames.AVERAGE_SEASON_EXPERIENCE_LABEL);
			BuildLabel(view.totalSeasonExperienceLabel, dataSource, 6, ClanSeasonRewardScreenComponentNames.TOTAL_SEASON_EXPERIENCE_LABEL);
			BuildLabel(view.clanNameLabel, dataSource, 1, ClanSeasonRewardScreenComponentNames.CLAN_NAME_LABEL_NAME);
		}

		private void BuildLabel(GameObject labelObject, ClanSeasonRewardDataSource dataSource, int dataIndex, string labelName)
		{
			BuiltComponentElements builtComponentElements = GenericComponentFactory.BuildComponent(GUIComponentType.TextLabel, dataSource, labelObject, makeInstance: false);
			builtComponentElements.componentController.SetName(labelName);
			builtComponentElements.componentController.Activate();
			(builtComponentElements.componentController as GenericTextLabelComponent).SetDataSourceIndex(dataIndex);
		}
	}
}
