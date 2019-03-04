using Simulation;
using Simulation.GUI;
using Simulation.SinglePlayer;
using Simulation.SinglePlayerCampaign;
using Simulation.SinglePlayerCampaign.DataTypes;
using Simulation.SinglePlayerCampaign.EntityViews;
using SinglePlayerCampaign.GUI.Simulation.EntityViews;
using SinglePlayerCampaign.Simulation.Components;
using SinglePlayerCampaign.Simulation.EntityDescriptors;
using SinglePlayerCampaign.Simulation.EntityViews;
using SinglePlayerServiceLayer;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class TrackCampaignMatchEngine : SingleEntityViewEngine<CampaignWaveVictoryCheckEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly ISinglePlayerRequestFactory _singlePlayerReqFactory;

		private readonly CurrentWaveObservable _currentWaveObservable;

		private readonly WeaponFireStateSync _weaponFireStateSync;

		private readonly IEntityFactory _entityFactory;

		private readonly IEntityFunctions _entityFunctions;

		private readonly DestructionManager _destructionManager;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		internal TrackCampaignMatchEngine(ISinglePlayerRequestFactory singlePlayerReqFactory, WeaponFireStateSync weaponFireStateSync, CurrentWaveObservable currentWaveObservable, IEntityFactory entityFactory, IEntityFunctions entityFunctions, DestructionManager destructionManager)
		{
			_singlePlayerReqFactory = singlePlayerReqFactory;
			_currentWaveObservable = currentWaveObservable;
			_weaponFireStateSync = weaponFireStateSync;
			_entityFactory = entityFactory;
			_entityFunctions = entityFunctions;
			_destructionManager = destructionManager;
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignWaveVictoryCheckEntityView entityView)
		{
			entityView.waveVictoryComponent.victoryAchieved.NotifyOnValueSet((Action<int, bool>)OnWaveVictory);
		}

		protected override void Remove(CampaignWaveVictoryCheckEntityView entityView)
		{
			entityView.waveVictoryComponent.victoryAchieved.StopNotify((Action<int, bool>)OnWaveVictory);
		}

		private void OnWaveVictory(int entityId, bool victoryAchieved)
		{
			if (victoryAchieved)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)PreStartNextWave);
			}
		}

		private IEnumerator PreStartNextWave()
		{
			ITimeComponent timeComponent = entityViewsDB.QueryEntityView<CampaignWaveUpdateTimeEntityView>(207).timeComponent;
			timeComponent.timeRunning.set_value(false);
			FasterListEnumerator<AIAgentDataComponentsNode> enumerator = entityViewsDB.QueryEntityViews<AIAgentDataComponentsNode>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AIAgentDataComponentsNode current = enumerator.get_Current();
					_destructionManager.DestroyMachine(current.aiBotIdData.playerId, current.aiBotIdData.playerId);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			ICounterComponent waveTracker = entityViewsDB.QueryEntityView<CurrentWaveTrackerEntityView>(208).CurrentWaveCounterComponent;
			int currentWaveIndex = waveTracker.counterValue.get_value();
			int nextWaveIndex = currentWaveIndex + 1;
			int numberOfWavesInThisCampaign = waveTracker.maxValue;
			bool lastWaveCompleted = nextWaveIndex == numberOfWavesInThisCampaign;
			UpdatePlayerCompletedCampaignWaveDependency updatePlayerCompletedCampaignWaveDependency = new UpdatePlayerCompletedCampaignWaveDependency(WorldSwitching.GetCampaignID(), WorldSwitching.GetCampaignDifficulty(), currentWaveIndex);
			IUpdatePlayerCompletedCampaignWaveRequest updatePlayerCompletedCampaignWaveReq = _singlePlayerReqFactory.Create<IUpdatePlayerCompletedCampaignWaveRequest>();
			updatePlayerCompletedCampaignWaveReq.Inject(updatePlayerCompletedCampaignWaveDependency);
			updatePlayerCompletedCampaignWaveReq.Execute();
			if (!lastWaveCompleted)
			{
				FasterReadOnlyList<CurrentWaveTrackerEntityView> currentWaveTrackerEntityViews = entityViewsDB.QueryEntityViews<CurrentWaveTrackerEntityView>();
				currentWaveTrackerEntityViews.get_Item(0).TransitionAnimationTriggerComponent.WaveCompleted.set_value(true);
				currentWaveTrackerEntityViews.get_Item(0).TransitionAnimationTriggerComponent.WaveCompleted.set_value(false);
				FasterReadOnlyList<WaveTransitionEntityView> waveTransitionsEntityViews = entityViewsDB.QueryEntityViews<WaveTransitionEntityView>();
				while (waveTransitionsEntityViews.get_Item(0).AnimationComponent.IsPlaying)
				{
					yield return null;
				}
				_weaponFireStateSync.ClearPendingData();
				FasterListEnumerator<HealthBarShowEntityView> enumerator2 = entityViewsDB.QueryEntityViews<HealthBarShowEntityView>().GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						HealthBarShowEntityView current2 = enumerator2.get_Current();
						current2.healthBarMachineIdComponent.isActive.set_value(false);
					}
				}
				finally
				{
					((IDisposable)enumerator2).Dispose();
				}
				WavesDataEntityView wavesDataEntity = entityViewsDB.QueryEntityView<WavesDataEntityView>(204);
				WaveData nextWaveData = wavesDataEntity.wavesData.wavesData.WavesData.get_Item(nextWaveIndex);
				_entityFunctions.RemoveEntity<CampaignWaveEntityDescriptor>(207);
				CampaignEntitiesFactory.BuildCampaignWaveEntity(_entityFactory, nextWaveData);
				WaveCounterInfo waveCounterInfo = new WaveCounterInfo(nextWaveIndex, numberOfWavesInThisCampaign);
				_currentWaveObservable.Dispatch(ref waveCounterInfo);
			}
			waveTracker.counterValue.set_value(nextWaveIndex);
		}
	}
}
