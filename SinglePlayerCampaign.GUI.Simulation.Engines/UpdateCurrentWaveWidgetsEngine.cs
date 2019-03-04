using SinglePlayerCampaign.GUI.Simulation.Components;
using SinglePlayerCampaign.GUI.Simulation.EntityViews;
using SinglePlayerCampaign.Simulation.Components;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.DataStructures;
using Svelto.ECS;
using System;

namespace SinglePlayerCampaign.GUI.Simulation.Engines
{
	internal class UpdateCurrentWaveWidgetsEngine : MultiEntityViewsEngine<CurrentWaveTrackerEntityView, CurrentWaveWidgetEntityView>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(CurrentWaveTrackerEntityView entityView)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			entityView.CurrentWaveCounterComponent.counterValue.NotifyOnValueSet((Action<int, int>)UpdateCurrentWaveWidgets);
			FasterListEnumerator<CurrentWaveWidgetEntityView> enumerator = entityViewsDB.QueryEntityViews<CurrentWaveWidgetEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CurrentWaveWidgetEntityView current = enumerator.get_Current();
					InitialiseWidget(entityView.CurrentWaveCounterComponent, current.CurrentWaveWidgetCounterComponent);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		protected override void Remove(CurrentWaveTrackerEntityView entityView)
		{
			entityView.CurrentWaveCounterComponent.counterValue.StopNotify((Action<int, int>)UpdateCurrentWaveWidgets);
		}

		protected override void Add(CurrentWaveWidgetEntityView entityView)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<CurrentWaveTrackerEntityView> val = entityViewsDB.QueryEntityViews<CurrentWaveTrackerEntityView>();
			if (val.get_Count() != 0)
			{
				InitialiseWidget(val.get_Item(0).CurrentWaveCounterComponent, entityView.CurrentWaveWidgetCounterComponent);
			}
		}

		protected override void Remove(CurrentWaveWidgetEntityView entityView)
		{
		}

		private static void InitialiseWidget(ICounterComponent tracker, IWidgetCounterComponent widget)
		{
			widget.WidgetCounterMaxValue = tracker.maxValue;
			widget.WidgetCounterValue = tracker.counterValue.get_value();
		}

		private void UpdateCurrentWaveWidgets(int entityID, int currentWaveIndex)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<CurrentWaveWidgetEntityView> enumerator = entityViewsDB.QueryEntityViews<CurrentWaveWidgetEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CurrentWaveWidgetEntityView current = enumerator.get_Current();
					current.CurrentWaveWidgetCounterComponent.WidgetCounterValue = currentWaveIndex;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
	}
}
