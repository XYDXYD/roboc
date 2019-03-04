using DG.Tweening;
using DG.Tweening.Core;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership.GUI
{
	internal class TierRobotRankingWidgetsEngine : SingleEntityViewEngine<RobotRankingWidgetsEntityView>, IQueryingEntityViewEngine, IWaitForFrameworkInitialization, IEngine
	{
		private readonly ICPUPower _cpuPower;

		private readonly IServiceRequestFactory _serviceReqFactory;

		private readonly LoadingIconPresenter _loadingIcon;

		private TiersData _tiersData;

		private int _currentRobotCPU;

		private int _currentRobotRanking;

		private int _currentRobotCosmeticCPU;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public TierRobotRankingWidgetsEngine(ICPUPower cpuPower, IServiceRequestFactory serviceReqFactory, LoadingIconPresenter loadingIcon)
		{
			_cpuPower = cpuPower;
			_serviceReqFactory = serviceReqFactory;
			_loadingIcon = loadingIcon;
		}

		public void Ready()
		{
		}

		public void OnFrameworkInitialized()
		{
			TaskRunner.get_Instance().Run(LoadTiersBanding());
		}

		protected override void Add(RobotRankingWidgetsEntityView entityView)
		{
			entityView.RobotRankingComponent.CubeRanking.NotifyOnValueSet((Action<int, RankingAndCPU>)UpdateRobotRankingAndCPU);
			InitializeWidgets();
		}

		protected override void Remove(RobotRankingWidgetsEntityView entityView)
		{
			entityView.RobotRankingComponent.CubeRanking.StopNotify((Action<int, RankingAndCPU>)UpdateRobotRankingAndCPU);
		}

		private IEnumerator LoadTiersBanding()
		{
			_loadingIcon.NotifyLoading("LoadingTiersData");
			ILoadTiersBandingRequest loadTiersBandingReq = _serviceReqFactory.Create<ILoadTiersBandingRequest>();
			TaskService<TiersData> loadTiersBandingTS = new TaskService<TiersData>(loadTiersBandingReq);
			HandleTaskServiceWithError handleTSWithError = new HandleTaskServiceWithError(loadTiersBandingTS, delegate
			{
				_loadingIcon.NotifyLoading("LoadingTiersData");
			}, delegate
			{
				_loadingIcon.NotifyLoadingDone("LoadingTiersData");
			});
			yield return handleTSWithError.GetEnumerator();
			_loadingIcon.NotifyLoadingDone("LoadingTiersData");
			if (loadTiersBandingTS.succeeded)
			{
				_tiersData = loadTiersBandingTS.result;
			}
		}

		private void UpdateRobotRankingAndCPU(int entityID, RankingAndCPU cubeRankingAndCpu)
		{
			_currentRobotCPU += cubeRankingAndCpu.TotalCPU;
			_currentRobotCosmeticCPU += cubeRankingAndCpu.TotalCosmeticCPU;
			_currentRobotRanking += cubeRankingAndCpu.Ranking;
			UpdateWidgets();
		}

		private void UpdateWidgets()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			RobotRankingWidgetsEntityView robotRankingWidgetsEntityView = entityViewsDB.QueryEntityViews<RobotRankingWidgetsEntityView>().get_Item(0);
			IRobotRankingWidgetComponent robotRankingWidgetComponent = robotRankingWidgetsEntityView.RobotRankingWidgetComponent;
			uint currentRobotRanking = (uint)_currentRobotRanking;
			float num = RRAndTiers.GetTiersCount(_tiersData);
			FasterReadOnlyList<TierWidgetEntityView> tierWidgetEntityViews = entityViewsDB.QueryEntityViews<TierWidgetEntityView>();
			UpdateRRLabelWidget(robotRankingWidgetComponent, currentRobotRanking);
			uint num2 = RobotCPUCalculator.CalculateRobotActualCPU((uint)_currentRobotCPU, (uint)_currentRobotCosmeticCPU, _cpuPower.MaxCosmeticCpuPool);
			bool flag = num2 > _cpuPower.MaxCpuPower;
			uint num3 = RRAndTiers.ConvertRRToTierIndex(currentRobotRanking, flag, _tiersData);
			float num5;
			if (!flag)
			{
				float num4 = (float)(double)num3 / num;
				num5 = num4;
				float num6 = (float)(double)RRAndTiers.GetTierLowerRRLimit(num3, _tiersData);
				float num7 = (float)(double)RRAndTiers.GetTierUpperRRLimit(num3, _tiersData);
				float num8 = ((float)(double)currentRobotRanking - num6) / (num7 - num6);
				num5 += num8 / num;
			}
			else
			{
				num5 = 1f;
			}
			UpdateRRSliderWidget(robotRankingWidgetComponent, num5);
			UpdateTierHighlightingWidgets(tierWidgetEntityViews, num3);
		}

		private void InitializeWidgets()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			RobotRankingWidgetsEntityView robotRankingWidgetsEntityView = entityViewsDB.QueryEntityViews<RobotRankingWidgetsEntityView>().get_Item(0);
			IRobotRankingWidgetComponent robotRankingWidgetComponent = robotRankingWidgetsEntityView.RobotRankingWidgetComponent;
			FasterReadOnlyList<TierWidgetEntityView> tierWidgetEntityViews = entityViewsDB.QueryEntityViews<TierWidgetEntityView>();
			UpdateRRLabelWidget(robotRankingWidgetComponent, 0u);
			UpdateRRSliderWidget(robotRankingWidgetComponent, 0f);
			UpdateTierHighlightingWidgets(tierWidgetEntityViews, 0u);
		}

		private static void UpdateRRLabelWidget(IRobotRankingWidgetComponent rrWidgetComponent, uint currentRobotRank)
		{
			rrWidgetComponent.RobotRankingAbs = currentRobotRank;
		}

		private unsafe static void UpdateRRSliderWidget(IRobotRankingWidgetComponent rrWidgetComponent, float endValue)
		{
			rrWidgetComponent.MainRRSequence = DOTween.Sequence();
			TweenSettingsExtensions.SetRecyclable<Sequence>(rrWidgetComponent.MainRRSequence, false);
			_003CUpdateRRSliderWidget_003Ec__AnonStorey1 _003CUpdateRRSliderWidget_003Ec__AnonStorey;
			DOGetter<float> val = new DOGetter<float>((object)_003CUpdateRRSliderWidget_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
			DOSetter<float> val2 = new DOSetter<float>((object)_003CUpdateRRSliderWidget_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
			TweenExtensions.Kill(rrWidgetComponent.MainRRSequence, false);
			Tween val3 = DOTween.To(val, val2, endValue, rrWidgetComponent.TweenDuration);
			TweenSettingsExtensions.Append(rrWidgetComponent.MainRRSequence, val3);
		}

		private static void UpdateTierHighlightingWidgets(FasterReadOnlyList<TierWidgetEntityView> tierWidgetEntityViews, uint tierIndex)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<TierWidgetEntityView> enumerator = tierWidgetEntityViews.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					TierWidgetEntityView current = enumerator.get_Current();
					ITierComponent tierComponent = current.TierComponent;
					tierComponent.tier = (int)tierIndex;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
	}
}
