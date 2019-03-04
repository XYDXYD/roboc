using Robocraft.GUI;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI
{
	internal class BattleExperienceScreenFactory
	{
		[Inject]
		internal IComponentFactory componentFactory
		{
			get;
			private set;
		}

		public void BuildAll(BattleExperienceView view, BattleExperienceDataSource battleDataSource)
		{
			BuildLabel(view.battleExperienceLabel, battleDataSource, BattleExperienceFields.BATTLE_XP);
			BuildLabel(view.partyBonusLabel, battleDataSource, BattleExperienceFields.PARTY_XP);
			BuildLabel(view.tierBonusLabel, battleDataSource, BattleExperienceFields.TIER_XP);
			BuildLabel(view.premiumExperienceLabel, battleDataSource, BattleExperienceFields.PREMIUM_XP);
			BuildLabel(view.totalLabel, battleDataSource, BattleExperienceFields.TOTAL_GAINED_XP);
			BuildLabel(view.playerRobitsLabel, battleDataSource, BattleExperienceFields.ROBITS);
			BuildLabel(view.playerPremiumRobitsLabel, battleDataSource, BattleExperienceFields.PREMIUM_ROBITS);
			BuildLabel(view.partyBonusCaptionLabel, battleDataSource, BattleExperienceFields.PARTY_XP_CAPTION);
			BuildLabel(view.premiumExperienceCaptionLabel, battleDataSource, BattleExperienceFields.PREMIUM_CAPTION);
			BuildLabel(view.tierBonusCaptionLabel, battleDataSource, BattleExperienceFields.TIER_XP_CAPTION);
			BuildLabel(view.playerPremiumRobitsCaptionLabel, battleDataSource, BattleExperienceFields.PREMIUM_CAPTION);
			BuildLabel(view.longPlayReductionCaptionLabel, battleDataSource, BattleExperienceFields.LONG_PLAY_REDUCTION_CAPTION);
			BuildLabel(view.longPlayReductionLabel, battleDataSource, BattleExperienceFields.LONG_PLAY_REDUCTION_ACTUAL_AMOUNT);
			BuildLabel(view.seasonExperienceLabel, battleDataSource, BattleExperienceFields.SEASON_XP);
			BuildLabel(view.clanAverageExperienceLabel, battleDataSource, BattleExperienceFields.CLAN_AVERAGE_XP);
			BuildLabel(view.clanTotalExperienceLabel, battleDataSource, BattleExperienceFields.CLAN_TOTAL_XP);
			BuildLabel(view.clanRobitsLabel, battleDataSource, BattleExperienceFields.CLAN_ROBITS);
			BattleExperienceLevelView componentInChildren = view.GetComponentInChildren<BattleExperienceLevelView>();
			componentInChildren.presenter.SetDataSource(battleDataSource);
		}

		private void BuildLabel(GameObject go, BattleExperienceDataSource dataSource, BattleExperienceFields dataField)
		{
			BuiltComponentElements builtComponentElements = componentFactory.BuildComponent(GUIComponentType.TextLabel, dataSource, go, makeInstance: false);
			GenericTextLabelComponent genericTextLabelComponent = (GenericTextLabelComponent)builtComponentElements.componentController;
			genericTextLabelComponent.Activate();
			genericTextLabelComponent.SetDataSourceIndex((int)dataField);
		}
	}
}
