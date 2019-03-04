using BehaviorDesigner.Runtime;
using Simulation.SinglePlayer;
using Svelto.DataStructures;
using Svelto.ECS;

namespace Simulation.AI
{
	public class AIWaypointEngine : SingleEntityViewEngine<AIAgentDataComponentsNode>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(AIAgentDataComponentsNode entityView)
		{
			SetWayPointsOnBehavior(entityView);
		}

		protected override void Remove(AIAgentDataComponentsNode entityView)
		{
		}

		private void SetWayPointsOnBehavior(AIAgentDataComponentsNode entityView)
		{
			PathNode[] waypoints = GetWaypoints(entityView.aiBotIdData.teamId);
			SharedVariable variable = entityView.agentBehaviorTreeComponent.aiAgentBehaviorTree.GetVariable("WaypointsLinkedList");
			variable.SetValue((object)waypoints);
		}

		private PathNode[] GetWaypoints(int teamId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<AIWaypointListNode> val = entityViewsDB.QueryEntityViews<AIWaypointListNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				AIWaypointListNode aIWaypointListNode = val.get_Item(i);
				if (aIWaypointListNode.aiWaypointListComponent.teamId == teamId)
				{
					return aIWaypointListNode.aiWaypointListComponent.nodes;
				}
			}
			return null;
		}
	}
}
