using EnginesGUI;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.GUI
{
	internal class VotingAfterBattleFactory
	{
		private const int ROBOT_WIDGETS_COUNT = 5;

		private EnginesRoot _enginesRoot;

		private Dictionary<VoteType, ThresholdData[]> _thresholds;

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IContainer container
		{
			get;
			private set;
		}

		[Inject]
		internal IGUIInputController guiInputController
		{
			get;
			private set;
		}

		[Inject]
		internal RegisterPlayerObserver registerPlayerObserver
		{
			get;
			private set;
		}

		[Inject]
		internal GameEndedObserver gameEndedObserver
		{
			get;
			private set;
		}

		[Inject]
		internal LoadingIconPresenter loadingPresenter
		{
			get;
			private set;
		}

		[Inject]
		internal IServiceRequestFactory requestFactory
		{
			get;
			private set;
		}

		[Inject]
		internal IEntityFactory entityFactory
		{
			get;
			private set;
		}

		public void Build(EnginesRoot root)
		{
			_enginesRoot = root;
			InGamePlayerStatsUpdatedObservable observable = container.Build<InGamePlayerStatsUpdatedObservable>();
			InGamePlayerStatsUpdatedObserver battleStatsUpdatedObserver = new InGamePlayerStatsUpdatedObserver(observable);
			UpdateVotingAfterBattleClientCommandObservable observable2 = container.Build<UpdateVotingAfterBattleClientCommandObservable>();
			UpdateVotingAfterBattleClientCommandObserver updateVotingAfterBattleClientCommandObserver = new UpdateVotingAfterBattleClientCommandObserver(observable2);
			VotingAfterBattleEngine votingAfterBattleEngine = container.Inject<VotingAfterBattleEngine>(new VotingAfterBattleEngine(battleStatsUpdatedObserver, registerPlayerObserver, updateVotingAfterBattleClientCommandObserver, gameEndedObserver, gameObjectFactory));
			_enginesRoot.AddEngine(votingAfterBattleEngine);
			VotingAfterBattleRobotEngine votingAfterBattleRobotEngine = container.Inject<VotingAfterBattleRobotEngine>(new VotingAfterBattleRobotEngine(gameEndedObserver));
			_enginesRoot.AddEngine(votingAfterBattleRobotEngine);
			guiInputController.AddDisplayScreens(new IGUIDisplay[1]
			{
				votingAfterBattleEngine
			});
			TaskRunner.get_Instance().Run(LoadVoteThresholds());
		}

		private void BuildImplementors()
		{
			GameObject val = gameObjectFactory.Build("VotingAfterBattle");
			VotingAfterBattleMainWindowImplementor componentInChildren = val.GetComponentInChildren<VotingAfterBattleMainWindowImplementor>();
			PanelSizeComponentImplementor componentInChildren2 = val.GetComponentInChildren<PanelSizeComponentImplementor>();
			entityFactory.BuildEntity<VotingAfterBattleMainWindowEntityDescriptor>(val.GetInstanceID(), new object[2]
			{
				componentInChildren,
				componentInChildren2
			});
			VotingAfterBattleRobotWidgetImplementor componentInChildren3 = val.GetComponentInChildren<VotingAfterBattleRobotWidgetImplementor>();
			for (int i = 0; i < 5; i++)
			{
				GameObject val2 = gameObjectFactory.Build(componentInChildren3.get_gameObject());
				VotingAfterBattleRobotWidgetImplementor component = val2.GetComponent<VotingAfterBattleRobotWidgetImplementor>();
				component.PedestalPosition = GetPedestalPositionByIndex(i);
				entityFactory.BuildEntity<VotingAfterBattleWidgetEntityDescriptor>(val2.GetInstanceID(), new object[1]
				{
					component
				});
				VotingAfterBattleRobotVoteImplementor[] componentsInChildren = val2.GetComponentsInChildren<VotingAfterBattleRobotVoteImplementor>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].robotWidgetID = val2.GetInstanceID();
					componentsInChildren[j].thresholds = _thresholds[componentsInChildren[j].type];
					entityFactory.BuildEntity<VotingAfterBattleVoteEntityDescriptor>(componentsInChildren[j].get_gameObject().GetInstanceID(), new object[1]
					{
						componentsInChildren[j]
					});
				}
				val2.get_transform().SetParent(componentInChildren.robotWidgetsContainer, false);
				UITable component2 = componentInChildren.robotWidgetsContainer.GetComponent<UITable>();
				component2.Reposition();
			}
			componentInChildren3.get_gameObject().SetActive(false);
		}

		private int GetPedestalPositionByIndex(int index)
		{
			int num = Mathf.FloorToInt(2.5f);
			if (index != num)
			{
				if (index > num)
				{
					return (index - num) * 2;
				}
				return (num - index) * 2 - 1;
			}
			return 0;
		}

		private IEnumerator LoadVoteThresholds()
		{
			loadingPresenter.NotifyLoading("LoadVotingAfterBattleThresholds");
			ILoadVotingAfterBattleThresholdsRequest request = requestFactory.Create<ILoadVotingAfterBattleThresholdsRequest>();
			TaskService<Dictionary<VoteType, ThresholdData[]>> task = new TaskService<Dictionary<VoteType, ThresholdData[]>>(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingPresenter.NotifyLoading("LoadVotingAfterBattleThresholds");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("LoadVotingAfterBattleThresholds");
			}).GetEnumerator();
			loadingPresenter.NotifyLoadingDone("LoadVotingAfterBattleThresholds");
			if (task.succeeded)
			{
				_thresholds = task.result;
				BuildImplementors();
			}
			else
			{
				OnLoadingFailed(task.behaviour);
			}
		}

		private void OnLoadingFailed(ServiceBehaviour behaviour)
		{
			ErrorWindow.ShowServiceErrorWindow(behaviour);
		}
	}
}
