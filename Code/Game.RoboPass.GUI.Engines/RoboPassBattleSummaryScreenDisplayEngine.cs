using DG.Tweening;
using DG.Tweening.Core;
using Fabric;
using Game.RoboPass.Components;
using Game.RoboPass.EntityViews;
using Game.RoboPass.GUI.Components;
using Game.RoboPass.GUI.EntityViews;
using Game.RoboPass.GUI.Implementors;
using Mothership;
using ServerUtilitiesShared.Utilities;
using Services.Analytics;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Game.RoboPass.GUI.Engines
{
	internal class RoboPassBattleSummaryScreenDisplayEngine : SingleEntityViewEngine<RoboPassBattleSummaryScreenEntityView>, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		private const string STR_CURR_GRADE = "strRoboPassBattleSummaryCurrentGrade";

		private readonly WorldSwitching _worldSwitching;

		private readonly IServiceRequestFactory _serviceRequestFactory;

		private readonly RobopassBattleSummaryScreenFactory _battleSummaryScreenFactory;

		private readonly Observable _roboPassBattleSummaryObservable;

		private readonly IGUIInputControllerMothership _guiInputController;

		private readonly ReloadRobopassObserver _reloadRobopassObserver;

		private readonly IAnalyticsRequestFactory _analyticsRequestFactory;

		private readonly WaitForSecondsEnumerator _waitBeforeAnimatingScreen = new WaitForSecondsEnumerator(1f);

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public RoboPassBattleSummaryScreenDisplayEngine(WorldSwitching worldSwitching, IServiceRequestFactory serviceRequestFactory, RobopassBattleSummaryScreenFactory battleSummaryScreenFactory, Observable roboPassBattleSummaryObservable, IGUIInputControllerMothership guiInputController, ReloadRobopassObserver reloadRobopassObserver, IAnalyticsRequestFactory analyticsRequestFactory)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			_worldSwitching = worldSwitching;
			_serviceRequestFactory = serviceRequestFactory;
			_battleSummaryScreenFactory = battleSummaryScreenFactory;
			_roboPassBattleSummaryObservable = roboPassBattleSummaryObservable;
			_guiInputController = guiInputController;
			_reloadRobopassObserver = reloadRobopassObserver;
			_analyticsRequestFactory = analyticsRequestFactory;
		}

		public void Ready()
		{
			_reloadRobopassObserver.AddAction((Action)OnRoboPassPurchased);
		}

		public void OnFrameworkDestroyed()
		{
			_reloadRobopassObserver.RemoveAction((Action)OnRoboPassPurchased);
		}

		protected override void Add(RoboPassBattleSummaryScreenEntityView entityView)
		{
			entityView.screenComponent.ContinueClicked.NotifyOnValueSet((Action<int, bool>)OnContinueClicked);
			entityView.screenComponent.OnScreenDisplayChange.NotifyOnValueSet((Action<int, bool>)OnScreenDisplayChange);
			entityView.rewardedItemsPanelComponent.CollectRewardsClicked.NotifyOnValueSet((Action<int, bool>)OnCollectRewards);
			entityView.rewardedItemsPanelComponent.ContinueClicked.NotifyOnValueSet((Action<int, bool>)OnContinueClicked);
			entityView.rewardedItemsPanelComponent.GetRoboPassPlusClicked.NotifyOnValueSet((Action<int, bool>)OnBuyNowClicked);
			entityView.rewardItemPurchaseComponent.SummaryPanelBuyNowClicked.NotifyOnValueSet((Action<int, bool>)OnBuyNowClicked);
			entityView.rewardItemPurchaseComponent.RewardsPanelBuyNowClicked.NotifyOnValueSet((Action<int, bool>)OnBuyNowClicked);
		}

		protected override void Remove(RoboPassBattleSummaryScreenEntityView entityView)
		{
			entityView.screenComponent.ContinueClicked.StopNotify((Action<int, bool>)OnContinueClicked);
			entityView.screenComponent.OnScreenDisplayChange.StopNotify((Action<int, bool>)OnScreenDisplayChange);
			entityView.rewardedItemsPanelComponent.CollectRewardsClicked.StopNotify((Action<int, bool>)OnCollectRewards);
			entityView.rewardedItemsPanelComponent.ContinueClicked.StopNotify((Action<int, bool>)OnContinueClicked);
			entityView.rewardedItemsPanelComponent.GetRoboPassPlusClicked.StopNotify((Action<int, bool>)OnBuyNowClicked);
			entityView.rewardedItemsPanelComponent.CollectRewardsClicked.StopNotify((Action<int, bool>)OnCollectRewards);
			entityView.rewardItemPurchaseComponent.SummaryPanelBuyNowClicked.StopNotify((Action<int, bool>)OnBuyNowClicked);
			entityView.rewardItemPurchaseComponent.RewardsPanelBuyNowClicked.StopNotify((Action<int, bool>)OnBuyNowClicked);
		}

		private void OnRoboPassPurchased()
		{
			_guiInputController.ForceCloseJustThisScreen(GuiScreens.BuyRoboPassAfterBattle);
			IRoboPassBattleSummaryRewardItemComponent rewardItemComponent = GetRewardItemComponent(RobopassRewardItemScreenType.BattleSummaryNextRewards, isDeluxe: true);
			IRoboPassBattleSummaryRewardItemComponent rewardItemComponent2 = GetRewardItemComponent(RobopassRewardItemScreenType.BattleSummaryRewardsPanel, isDeluxe: true);
			rewardItemComponent.IsLocked = false;
			rewardItemComponent2.IsLocked = false;
		}

		private IRoboPassBattleSummaryRewardItemComponent GetRewardItemComponent(RobopassRewardItemScreenType screenType, bool isDeluxe)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<RoboPassBattleSummaryRewardItemEntityView> val = entityViewsDB.QueryEntityViews<RoboPassBattleSummaryRewardItemEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				IRoboPassBattleSummaryRewardItemComponent rewardItemComponent = val.get_Item(i).rewardItemComponent;
				if (rewardItemComponent.ItemScreenType == screenType && rewardItemComponent.IsDeluxe == isDeluxe)
				{
					return rewardItemComponent;
				}
			}
			return null;
		}

		private void OnContinueClicked(int id, bool value)
		{
			TaskRunner.get_Instance().Run(MarkProgressAsShown());
			_roboPassBattleSummaryObservable.Dispatch();
		}

		private void OnCollectRewards(int id, bool value)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			RoboPassBattleSummaryScreenEntityView roboPassBattleSummaryScreenEntityView = entityViewsDB.QueryEntityViews<RoboPassBattleSummaryScreenEntityView>().get_Item(0);
			roboPassBattleSummaryScreenEntityView.rewardedItemsPanelComponent.PanelActive = false;
		}

		private void OnScreenDisplayChange(int id, bool active)
		{
			RoboPassBattleSummaryScreenEntityView roboPassBattleSummaryScreenEntityView = entityViewsDB.QueryEntityView<RoboPassBattleSummaryScreenEntityView>(id);
			if (!active)
			{
				roboPassBattleSummaryScreenEntityView.screenComponent.ScreenActive = false;
			}
			else
			{
				TaskRunner.get_Instance().Run(DisplayScreen(roboPassBattleSummaryScreenEntityView));
			}
		}

		private IEnumerator DisplayScreen(RoboPassBattleSummaryScreenEntityView battleSummaryScreenEntity)
		{
			ILoadRoboPassSeasonConfigRequest loadRoboPassSeasonConfigReq = _serviceRequestFactory.Create<ILoadRoboPassSeasonConfigRequest>();
			loadRoboPassSeasonConfigReq.ClearCache();
			TaskService<RoboPassSeasonData> loadRoboPassSeasonConfigTS = loadRoboPassSeasonConfigReq.AsTask();
			yield return loadRoboPassSeasonConfigTS;
			if (!loadRoboPassSeasonConfigTS.succeeded)
			{
				throw new Exception("Failed to get RoboPass season data");
			}
			RoboPassSeasonData roboPassSeasonData = loadRoboPassSeasonConfigTS.result;
			RoboPassSeasonDataEntityView seasonDataEV = entityViewsDB.QueryEntityViews<RoboPassSeasonDataEntityView>().get_Item(0);
			IRoboPassSeasonInfoComponent roboPassSeasonInfoComp = seasonDataEV.roboPassSeasonInfoComponent;
			if (roboPassSeasonData == null)
			{
				roboPassSeasonInfoComp.isValidSeason = false;
			}
			else
			{
				TimeSpan t = roboPassSeasonInfoComp.timeRemaining = roboPassSeasonData.endDateTimeUTC - DateTime.Now;
				roboPassSeasonInfoComp.isValidSeason = (t > TimeSpan.Zero);
			}
			if (CanShowScreen())
			{
				yield return ReloadPlayerInfoData(seasonDataEV);
				yield return ShowScreen(battleSummaryScreenEntity, seasonDataEV);
			}
			else
			{
				_roboPassBattleSummaryObservable.Dispatch();
			}
		}

		private IEnumerator ReloadPlayerInfoData(RoboPassSeasonDataEntityView seasonDataEntityView)
		{
			ILoadPlayerRoboPassSeasonRequest loadPlayerRoboPassSeasonReq = _serviceRequestFactory.Create<ILoadPlayerRoboPassSeasonRequest>();
			loadPlayerRoboPassSeasonReq.ClearCache();
			TaskService<PlayerRoboPassSeasonData> loadPlayerRoboPassSeasonTS = loadPlayerRoboPassSeasonReq.AsTask();
			yield return loadPlayerRoboPassSeasonTS;
			PlayerRoboPassSeasonData playerRoboPassSeasonData = loadPlayerRoboPassSeasonTS.result;
			if (playerRoboPassSeasonData == null)
			{
				playerRoboPassSeasonData = new PlayerRoboPassSeasonData();
			}
			IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerInfoComponent = seasonDataEntityView.roboPassSeasonPlayerInfoComponent;
			roboPassSeasonPlayerInfoComponent.currentGradeIndex = playerRoboPassSeasonData.currentGradeIndex;
			roboPassSeasonPlayerInfoComponent.deltaXPToShow = playerRoboPassSeasonData.deltaXpToShow;
			roboPassSeasonPlayerInfoComponent.hasDeluxe = playerRoboPassSeasonData.hasDeluxe;
			roboPassSeasonPlayerInfoComponent.progressInGrade = playerRoboPassSeasonData.progressInGrade;
			roboPassSeasonPlayerInfoComponent.dataUpdated.set_value(true);
		}

		private bool CanShowScreen()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			RoboPassSeasonDataEntityView roboPassSeasonDataEntityView = entityViewsDB.QueryEntityViews<RoboPassSeasonDataEntityView>().get_Item(0);
			IRoboPassSeasonInfoComponent roboPassSeasonInfoComponent = roboPassSeasonDataEntityView.roboPassSeasonInfoComponent;
			IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerInfoComponent = roboPassSeasonDataEntityView.roboPassSeasonPlayerInfoComponent;
			bool flag = roboPassSeasonPlayerInfoComponent.deltaXPToShow > 0;
			bool flag2 = _worldSwitching.SwitchingFromSimulation && WorldSwitching.GetGameModeType() != GameModeType.TestMode;
			bool isValidSeason = roboPassSeasonInfoComponent.isValidSeason;
			int num = roboPassSeasonPlayerInfoComponent.currentGradeIndex + 1;
			int num2 = roboPassSeasonInfoComponent.xpBetweenGrades.Length;
			bool flag3 = num == num2;
			return isValidSeason && !flag3 && (flag2 || flag);
		}

		private void OnBuyNowClicked(int id, bool active)
		{
			if (active)
			{
				PurchaseFunnelHelper.SendEvent(_analyticsRequestFactory, "RoboPassPrompt", "BattleSummary", startsNewChain: true);
				Application.OpenURL("https://rc.qq.com/client/r2.htm");
			}
		}

		private IEnumerator MarkProgressAsShown()
		{
			IMarkPlayerRoboPassSeasonProgressAsSeenRequest markPlayerRoboPassSeasonProgressAsSeenReq = _serviceRequestFactory.Create<IMarkPlayerRoboPassSeasonProgressAsSeenRequest>();
			TaskService markPlayerRoboPassSeasonProgressAsSeenTS = markPlayerRoboPassSeasonProgressAsSeenReq.AsTask();
			yield return markPlayerRoboPassSeasonProgressAsSeenTS;
			if (markPlayerRoboPassSeasonProgressAsSeenTS.succeeded)
			{
				IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerInfoComponent = entityViewsDB.QueryEntityViews<RoboPassSeasonDataEntityView>().get_Item(0).roboPassSeasonPlayerInfoComponent;
				roboPassSeasonPlayerInfoComponent.deltaXPToShow = 0;
				roboPassSeasonPlayerInfoComponent.dataUpdated.set_value(true);
				yield break;
			}
			throw new Exception("Failed to get RoboPass player season data");
		}

		private IEnumerator ShowScreen(RoboPassBattleSummaryScreenEntityView battleSummaryScreenEntity, RoboPassSeasonDataEntityView seasonDataEntityView)
		{
			IRoboPassSeasonInfoComponent roboPassSeasonInfoComponent = seasonDataEntityView.roboPassSeasonInfoComponent;
			IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerInfoComponent = seasonDataEntityView.roboPassSeasonPlayerInfoComponent;
			IBattleSummaryNextGradeRewardsComponent nextGradeRewardsComponent = battleSummaryScreenEntity.nextGradeRewardsComponent;
			IBattleSummaryRewardedItemsPanelComponent rewardedItemsPanelComponent = battleSummaryScreenEntity.rewardedItemsPanelComponent;
			IBattleSummaryScreenComponent battleSummaryScreenComponent = battleSummaryScreenEntity.screenComponent;
			IBattleSummaryAnimationComponent battleSummaryAnimationComponent = battleSummaryScreenEntity.battleSummaryAnimationComponent;
			float deltaProgress = CalculateDeltaProgress(roboPassSeasonInfoComponent, roboPassSeasonPlayerInfoComponent);
			float oldProgress = roboPassSeasonPlayerInfoComponent.progressInGrade;
			float updatedProgress = oldProgress + deltaProgress;
			int currentGradeIndex = roboPassSeasonPlayerInfoComponent.currentGradeIndex;
			int nextGradeIndex = currentGradeIndex + 1;
			int gradesHighestIndex = roboPassSeasonInfoComponent.gradesHighestIndex;
			bool hasDeluxe = roboPassSeasonPlayerInfoComponent.hasDeluxe;
			bool hasUserReachedNextGrade = HasUserReachedNextGrade(gradesHighestIndex, currentGradeIndex, updatedProgress);
			battleSummaryScreenComponent.CurrentSeasonLabel = StringTableBase<StringTable>.Instance.GetString(roboPassSeasonInfoComponent.robopassSeasonName);
			battleSummaryScreenComponent.CurrentGradeLabel = string.Format(StringTableBase<StringTable>.Instance.GetString("strRoboPassBattleSummaryCurrentGrade"), currentGradeIndex + 1);
			battleSummaryAnimationComponent.CurrentProgress = oldProgress;
			battleSummaryAnimationComponent.NewProgress = oldProgress;
			UpdateNextRewardsUI(nextGradeRewardsComponent, roboPassSeasonInfoComponent.gradesRewards, currentGradeIndex, gradesHighestIndex, hasDeluxe);
			battleSummaryScreenComponent.ContinueButtonActive = !hasUserReachedNextGrade;
			float newGradeProgress = 0f;
			int newGradeIndex = 0;
			if (hasUserReachedNextGrade)
			{
				roboPassSeasonPlayerInfoComponent.reachedNewGrade.set_value(true);
				newGradeProgress = updatedProgress - 1f;
				newGradeIndex = (roboPassSeasonPlayerInfoComponent.currentGradeIndex = nextGradeIndex);
				roboPassSeasonPlayerInfoComponent.progressInGrade = newGradeProgress;
			}
			else
			{
				roboPassSeasonPlayerInfoComponent.progressInGrade = updatedProgress;
			}
			roboPassSeasonPlayerInfoComponent.dataUpdated.set_value(true);
			battleSummaryScreenEntity.screenComponent.ScreenActive = true;
			_waitBeforeAnimatingScreen.Reset();
			yield return _waitBeforeAnimatingScreen;
			if (hasUserReachedNextGrade)
			{
				SetNewProgressTweenTo(1f, battleSummaryAnimationComponent);
				while (battleSummaryAnimationComponent.NewProgress < 1f)
				{
					yield return null;
				}
				yield return PlayAndWaitAnimation(battleSummaryScreenComponent.Animation, battleSummaryScreenComponent.LevelUpAnimationPart1);
				battleSummaryAnimationComponent.CurrentProgress = 0f;
				battleSummaryAnimationComponent.NewProgress = 0f;
				if (currentGradeIndex < gradesHighestIndex)
				{
					SetNewProgressTweenTo(newGradeProgress, battleSummaryAnimationComponent);
				}
				battleSummaryScreenComponent.CurrentGradeLabel = string.Format(StringTableBase<StringTable>.Instance.GetString("strRoboPassBattleSummaryCurrentGrade"), newGradeIndex + 1);
				UpdateNextRewardsUI(nextGradeRewardsComponent, roboPassSeasonInfoComponent.gradesRewards, newGradeIndex, gradesHighestIndex, hasDeluxe);
				yield return PlayAndWaitAnimation(battleSummaryScreenComponent.Animation, battleSummaryScreenComponent.LevelUpAnimationPart2);
				ShowRewardsCollectPanel(rewardedItemsPanelComponent, roboPassSeasonInfoComponent.gradesRewards[newGradeIndex], roboPassSeasonPlayerInfoComponent.hasDeluxe);
				battleSummaryScreenComponent.ContinueButtonActive = true;
			}
			else
			{
				SetNewProgressTweenTo(updatedProgress, battleSummaryAnimationComponent);
			}
		}

		private void ShowRewardsCollectPanel(IBattleSummaryRewardedItemsPanelComponent rewardedItemsPanelComponent, IList<RoboPassSeasonRewardData> rewardsToGive, bool hasDeluxe)
		{
			if (!hasDeluxe && rewardsToGive.Count == 1 && rewardsToGive[0].isDeluxe)
			{
				rewardedItemsPanelComponent.ButtonCollectActive = false;
				rewardedItemsPanelComponent.ButtonContinueActive = true;
				rewardedItemsPanelComponent.ButtonGetRoboPassPlusActive = true;
				rewardedItemsPanelComponent.RoboPassPlusRewardTextActive = false;
				rewardedItemsPanelComponent.DescMsg = rewardedItemsPanelComponent.DescRewardUnlocked;
				rewardedItemsPanelComponent.TitleMsg = rewardedItemsPanelComponent.TitleRewardUnlocked;
			}
			else
			{
				rewardedItemsPanelComponent.ButtonCollectActive = true;
				rewardedItemsPanelComponent.ButtonContinueActive = false;
				rewardedItemsPanelComponent.ButtonGetRoboPassPlusActive = false;
				rewardedItemsPanelComponent.RoboPassPlusRewardTextActive = true;
				rewardedItemsPanelComponent.DescMsg = rewardedItemsPanelComponent.DescRewardReceived;
				rewardedItemsPanelComponent.TitleMsg = rewardedItemsPanelComponent.TitleRewardReceived;
			}
			IRoboPassBattleSummaryRewardItemComponent rewardItemComponent = GetRewardItemComponent(RobopassRewardItemScreenType.BattleSummaryRewardsPanel, isDeluxe: false);
			IRoboPassBattleSummaryRewardItemComponent rewardItemComponent2 = GetRewardItemComponent(RobopassRewardItemScreenType.BattleSummaryRewardsPanel, isDeluxe: true);
			rewardItemComponent.ItemActive = false;
			rewardItemComponent2.ItemActive = false;
			foreach (RoboPassSeasonRewardData item in rewardsToGive)
			{
				if (item.isDeluxe)
				{
					rewardItemComponent2.ItemName = item.rewardName;
					rewardItemComponent2.IsSpriteFullSize = item.spriteFullSize;
					rewardItemComponent2.ItemSprite = item.spriteName;
					rewardItemComponent2.ItemType = item.categoryName;
					rewardItemComponent2.IsLocked = !hasDeluxe;
					rewardItemComponent2.ItemActive = true;
				}
				else
				{
					rewardItemComponent.ItemName = item.rewardName;
					rewardItemComponent.IsSpriteFullSize = item.spriteFullSize;
					rewardItemComponent.ItemSprite = item.spriteName;
					rewardItemComponent.ItemType = item.categoryName;
					rewardItemComponent.ItemActive = true;
				}
			}
			rewardedItemsPanelComponent.RewardsGrid.Reposition();
			rewardedItemsPanelComponent.PanelActive = true;
		}

		private void UpdateNextRewardsUI(IBattleSummaryNextGradeRewardsComponent nextGradeRewardsComp, IList<RoboPassSeasonRewardData[]> gradesRewards, int currentGradeIndex, int gradesHighestIndex, bool hasDeluxe)
		{
			nextGradeRewardsComp.NextGradeRewardsLabelActive = false;
			IRoboPassBattleSummaryRewardItemComponent rewardItemComponent = GetRewardItemComponent(RobopassRewardItemScreenType.BattleSummaryNextRewards, isDeluxe: false);
			IRoboPassBattleSummaryRewardItemComponent rewardItemComponent2 = GetRewardItemComponent(RobopassRewardItemScreenType.BattleSummaryNextRewards, isDeluxe: true);
			rewardItemComponent.ItemActive = false;
			rewardItemComponent2.ItemActive = false;
			if (currentGradeIndex >= gradesHighestIndex)
			{
				return;
			}
			int index = currentGradeIndex + 1;
			RoboPassSeasonRewardData[] array = gradesRewards[index];
			if (array == null || array.Length <= 0)
			{
				return;
			}
			nextGradeRewardsComp.NextGradeRewardsLabelActive = true;
			RoboPassSeasonRewardData[] array2 = array;
			foreach (RoboPassSeasonRewardData roboPassSeasonRewardData in array2)
			{
				if (roboPassSeasonRewardData.isDeluxe)
				{
					rewardItemComponent2.ItemName = roboPassSeasonRewardData.rewardName;
					rewardItemComponent2.IsSpriteFullSize = roboPassSeasonRewardData.spriteFullSize;
					rewardItemComponent2.ItemSprite = roboPassSeasonRewardData.spriteName;
					rewardItemComponent2.ItemType = roboPassSeasonRewardData.categoryName;
					rewardItemComponent2.IsLocked = !hasDeluxe;
					rewardItemComponent2.ItemActive = true;
				}
				else
				{
					rewardItemComponent.ItemName = roboPassSeasonRewardData.rewardName;
					rewardItemComponent.IsSpriteFullSize = roboPassSeasonRewardData.spriteFullSize;
					rewardItemComponent.ItemSprite = roboPassSeasonRewardData.spriteName;
					rewardItemComponent.ItemType = roboPassSeasonRewardData.categoryName;
					rewardItemComponent.ItemActive = true;
				}
			}
			nextGradeRewardsComp.NextRewardsGrid.Reposition();
		}

		private static IEnumerator PlayAndWaitAnimation(Animation animation, string animationName)
		{
			animation.Play(animationName);
			while (animation.get_isPlaying())
			{
				yield return null;
			}
		}

		private static bool HasUserReachedNextGrade(int gradesHighestIndex, int currentGrade, float updatedProgress)
		{
			return gradesHighestIndex > currentGrade && updatedProgress >= 1f;
		}

		private static float CalculateDeltaProgress(IRoboPassSeasonInfoComponent roboPassSeasonInfoComponent, IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerInfoComponent)
		{
			int num = roboPassSeasonInfoComponent.xpBetweenGrades[roboPassSeasonPlayerInfoComponent.currentGradeIndex];
			int deltaXPToShow = roboPassSeasonPlayerInfoComponent.deltaXPToShow;
			float num2 = (float)deltaXPToShow / (float)num;
			float num3 = FloatUtils.FindMaximumPrecisionInRange(1, num);
			if (deltaXPToShow > 0 && num2 < num3)
			{
				num2 = num3;
			}
			float progressInGrade = roboPassSeasonPlayerInfoComponent.progressInGrade;
			float num4 = progressInGrade + num2;
			if (num4 >= 2f)
			{
				Console.LogError("Reached more than 1 grade. If the designer set the right values in the json this should not happen!");
				int upperBound = roboPassSeasonInfoComponent.xpBetweenGrades[roboPassSeasonPlayerInfoComponent.currentGradeIndex + 1];
				float num5 = FloatUtils.FindMaximumPrecisionInRange(1, upperBound);
				num2 = 2f - progressInGrade - num5;
			}
			return num2;
		}

		private unsafe void SetNewProgressTweenTo(float value, IBattleSummaryAnimationComponent battleSummaryAnimationComponent)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			_003CSetNewProgressTweenTo_003Ec__AnonStorey5 _003CSetNewProgressTweenTo_003Ec__AnonStorey;
			Tween val = DOTween.To(new DOGetter<float>((object)_003CSetNewProgressTweenTo_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)_003CSetNewProgressTweenTo_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), value, battleSummaryAnimationComponent.NewProgressTweenDuration);
			StartFillSound();
			TweenSettingsExtensions.OnUpdate<Tween>(val, new TweenCallback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			TweenSettingsExtensions.OnComplete<Tween>(val, new TweenCallback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void StartFillSound()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			GameObject gameObject = entityViewsDB.QueryEntityViews<RoboPassBattleSummaryScreenEntityView>().get_Item(0).battleSummaryAnimationComponent.GameObject;
			EventManager.get_Instance().PostEvent("GUI_ProgressBar_Loop", 0, (object)null, gameObject);
		}

		private void StopFillSound()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			GameObject gameObject = entityViewsDB.QueryEntityViews<RoboPassBattleSummaryScreenEntityView>().get_Item(0).battleSummaryAnimationComponent.GameObject;
			EventManager.get_Instance().PostEvent("GUI_ProgressBar_Loop", 1, (object)null, gameObject);
		}

		private void ModulateFillSound()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<RoboPassBattleSummaryScreenEntityView> val = entityViewsDB.QueryEntityViews<RoboPassBattleSummaryScreenEntityView>();
			GameObject gameObject = val.get_Item(0).battleSummaryAnimationComponent.GameObject;
			float newProgress = val.get_Item(0).battleSummaryAnimationComponent.NewProgress;
			EventManager.get_Instance().SetParameter("GUI_ProgressBar_Loop", "FILL", newProgress, gameObject);
		}
	}
}
