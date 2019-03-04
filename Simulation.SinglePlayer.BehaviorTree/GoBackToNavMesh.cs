using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Utility;

namespace Simulation.SinglePlayer.BehaviorTree
{
	public class GoBackToNavMesh : Action
	{
		public SharedAIBehaviorTreeAgentNode agent;

		public SharedBool willingToMove;

		private Vector3 goal;

		private float MAX_SAMPLING_DISTANCE = 20f;

		public GoBackToNavMesh()
			: this()
		{
		}

		public override void OnStart()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			Console.LogWarning("Going back to navmesh " + agent.get_Value().aiMovementData.playerName);
			NavMeshHit val = default(NavMeshHit);
			if (NavMesh.SamplePosition(agent.get_Value().aiMovementData.position, ref val, MAX_SAMPLING_DISTANCE, -1))
			{
				goal = val.get_position();
			}
			else
			{
				goal = agent.get_Value().aiMovementData.position;
			}
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			AIMovementUtils.MoveForwardTowardsTarget(agent.get_Value().aiMovementData.position, goal, agent.get_Value());
			return 3;
		}
	}
}
