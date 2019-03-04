using RCNetwork.Events;
using Simulation.Hardware;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;

namespace Simulation.SinglePlayer.AlignmentRectifier
{
	internal class AIAlignmentRectifierEngine : SingleEntityViewEngine<AIAgentDataComponentsNode>, IPhysicallyTickable, IInitialize, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private FasterList<AIAgentDataComponentsNode> _agents = new FasterList<AIAgentDataComponentsNode>();

		private Dictionary<int, AIAlignmentRectifierBehavior> _alignmentRectifierBehaviors = new Dictionary<int, AIAlignmentRectifierBehavior>();

		private float _time;

		[Inject]
		public INetworkEventManagerClient eventManagerClient
		{
			private get;
			set;
		}

		[Inject]
		public DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		public PlayerNamesContainer playerNamesContainer
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(AIAgentDataComponentsNode node)
		{
			_agents.Add(node);
			_alignmentRectifierBehaviors[node.get_ID()] = new AIAlignmentRectifierBehavior(node, node.get_ID());
		}

		protected override void Remove(AIAgentDataComponentsNode node)
		{
			_agents.UnorderedRemove(node);
			_alignmentRectifierBehaviors.Remove(node.get_ID());
		}

		public void PhysicsTick(float deltaSec)
		{
			_time += deltaSec;
			for (int i = 0; i < _agents.get_Count(); i++)
			{
				AIAgentDataComponentsNode aIAgentDataComponentsNode = _agents.get_Item(i);
				if (aIAgentDataComponentsNode.aiAlignmentRectifierControlComponent.pulseAR && !aIAgentDataComponentsNode.aiAlignmentRectifierControlComponent.alignmentRectifierExecuting && _time - aIAgentDataComponentsNode.aiAlignmentRectifierControlComponent.lastCompletedAlignmentRectifierTimestamp > 1f)
				{
					aIAgentDataComponentsNode.aiAlignmentRectifierControlComponent.alignmentRectifierExecuting = true;
					AIAlignmentRectifierBehavior aIAlignmentRectifierBehavior = _alignmentRectifierBehaviors[aIAgentDataComponentsNode.get_ID()];
					aIAlignmentRectifierBehavior.OnAlignmentRectifierInterrupted += HandleAlignmentComplete;
					aIAlignmentRectifierBehavior.Activate(3f);
					int value = aIAgentDataComponentsNode.get_ID();
					aIAgentDataComponentsNode.rectifyingComponent.functionalsEnabled = false;
					aIAgentDataComponentsNode.aiAlignmentRectifierControlComponent.alignmentRectifierStarted.Dispatch(ref value);
					PlayerIdDependency dependency = new PlayerIdDependency(aIAgentDataComponentsNode.aiBotIdData.playerId);
					eventManagerClient.SendEventToServer(NetworkEvent.AlignmentRectifierStarted, dependency);
				}
				if (aIAgentDataComponentsNode.aiAlignmentRectifierControlComponent.alignmentRectifierExecuting)
				{
					AIAlignmentRectifierBehavior aIAlignmentRectifierBehavior2 = _alignmentRectifierBehaviors[aIAgentDataComponentsNode.get_ID()];
					UpdateAgentInput(aIAgentDataComponentsNode, aIAlignmentRectifierBehavior2);
					aIAlignmentRectifierBehavior2.PhysicsUpdate(deltaSec);
					aIAlignmentRectifierBehavior2.TimeTick(deltaSec);
					if (aIAlignmentRectifierBehavior2.GetElapsedTime() >= 3f)
					{
						HandleAlignmentComplete(aIAlignmentRectifierBehavior2);
					}
				}
			}
		}

		private void HandleOnMachineDestroyed(int playerId, int machineId, bool isMe)
		{
			if (_alignmentRectifierBehaviors.TryGetValue(machineId, out AIAlignmentRectifierBehavior value))
			{
				value.GoIdle();
			}
		}

		private void HandleAlignmentComplete(AIAlignmentRectifierBehavior ailgnmentRectifierBehavior)
		{
			ailgnmentRectifierBehavior.OnAlignmentRectifierInterrupted -= HandleAlignmentComplete;
			ailgnmentRectifierBehavior.GoIdle();
			ailgnmentRectifierBehavior.aiAlignmentRectifierControlComponent.alignmentRectifierExecuting = false;
			ailgnmentRectifierBehavior.aiAlignmentRectifierControlComponent.alignmentRectifierComplete.Dispatch(ref _time);
			MachineRectifierNode machineRectifierNode = default(MachineRectifierNode);
			if (entityViewsDB.TryQueryEntityView<MachineRectifierNode>(ailgnmentRectifierBehavior.machineId, ref machineRectifierNode))
			{
				machineRectifierNode.machineFunctionalsComponent.functionalsEnabled = true;
			}
		}

		private static void UpdateAgentInput(AIAgentDataComponentsNode agentNode, AIAlignmentRectifierBehavior ailgnmentRectifierBehavior)
		{
			ailgnmentRectifierBehavior.ResetInputSignal();
			if (agentNode.aiAlignmentRectifierControlComponent.horizontalAxis > 0f)
			{
				ailgnmentRectifierBehavior.RightSignal();
			}
			if (agentNode.aiAlignmentRectifierControlComponent.horizontalAxis < 0f)
			{
				ailgnmentRectifierBehavior.LeftSignal();
			}
			if (agentNode.aiAlignmentRectifierControlComponent.forwardAxis > 0f)
			{
				ailgnmentRectifierBehavior.ForwardSignal();
			}
			if (agentNode.aiAlignmentRectifierControlComponent.forwardAxis < 0f)
			{
				ailgnmentRectifierBehavior.BackSignal();
			}
		}

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnMachineDestroyed += HandleOnMachineDestroyed;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineDestroyed -= HandleOnMachineDestroyed;
		}
	}
}
