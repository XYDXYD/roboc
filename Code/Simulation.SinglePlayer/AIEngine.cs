using BehaviorDesigner.Runtime;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Simulation.SinglePlayer
{
	internal class AIEngine : MultiEntityViewsEngine<AIAgentDataComponentsNode, MachineTargetsEntityView>, IQueryingEntityViewEngine, IInitialize, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IEngine
	{
		private AllowMovementObserver _allowMovementObserver;

		private ITaskRoutine _tickTask;

		[Inject]
		internal GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public AIEngine(AllowMovementObserver allowMovementObserver)
		{
			_allowMovementObserver = allowMovementObserver;
		}

		public void Ready()
		{
		}

		public void OnDependenciesInjected()
		{
			gameEndedObserver.OnGameEnded += HandleGameEnded;
		}

		public unsafe void OnFrameworkInitialized()
		{
			_allowMovementObserver.AddAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_tickTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetScheduler(StandardSchedulers.get_physicScheduler())
				.SetEnumeratorProvider((Func<IEnumerator>)Tick);
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_allowMovementObserver.RemoveAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			gameEndedObserver.OnGameEnded -= HandleGameEnded;
			_tickTask.Stop();
		}

		protected override void Add(AIAgentDataComponentsNode entityView)
		{
			SharedVariable variable = entityView.agentBehaviorTreeComponent.aiAgentBehaviorTree.GetVariable("AgentDataNode");
			variable.SetValue((object)entityView);
			MachineTargetsEntityView entityView2 = default(MachineTargetsEntityView);
			if (entityViewsDB.TryQueryEntityView<MachineTargetsEntityView>(entityView.get_ID(), ref entityView2))
			{
				PassEntityViewToBehaviorTree(entityView, entityView2);
			}
		}

		protected override void Remove(AIAgentDataComponentsNode entityView)
		{
		}

		protected override void Add(MachineTargetsEntityView entityView)
		{
			AIAgentDataComponentsNode agentData = default(AIAgentDataComponentsNode);
			if (entityViewsDB.TryQueryEntityView<AIAgentDataComponentsNode>(entityView.get_ID(), ref agentData))
			{
				PassEntityViewToBehaviorTree(agentData, entityView);
			}
		}

		protected override void Remove(MachineTargetsEntityView entityView)
		{
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				TickBehaviorTrees();
				yield return null;
			}
		}

		private void TickBehaviorTrees()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<AIAgentDataComponentsNode> val = entityViewsDB.QueryEntityViews<AIAgentDataComponentsNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				BehaviorManager.instance.Tick(val.get_Item(i).agentBehaviorTreeComponent.aiAgentBehaviorTree);
			}
		}

		private void HandleGameEnded(bool victory)
		{
			DisableAIs();
		}

		private void AllowMovementChanged(ref bool allowMovement)
		{
			if (allowMovement)
			{
				ActivateAIs();
			}
			else
			{
				DisableAIs();
			}
		}

		private void ActivateAIs()
		{
			_tickTask.Start((Action<PausableTaskException>)null, (Action)null);
		}

		private void DisableAIs()
		{
			_tickTask.Stop();
			ResetInput();
		}

		private void ResetInput()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<AIAgentDataComponentsNode> val = entityViewsDB.QueryEntityViews<AIAgentDataComponentsNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				AIAgentDataComponentsNode aIAgentDataComponentsNode = val.get_Item(i);
				aIAgentDataComponentsNode.aiInputWrapper.Reset();
			}
		}

		private void PassEntityViewToBehaviorTree(AIAgentDataComponentsNode agentData, MachineTargetsEntityView entityView)
		{
			SharedVariable variable = agentData.agentBehaviorTreeComponent.aiAgentBehaviorTree.GetVariable("MachineTargetsData");
			variable.SetValue((object)entityView);
		}
	}
}
