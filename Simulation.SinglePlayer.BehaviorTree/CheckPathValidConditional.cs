using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Simulation.SinglePlayer.CapturePoints;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Simulation.SinglePlayer.BehaviorTree
{
	public class CheckPathValidConditional : Conditional
	{
		public const float BAKED_AGENT_HEIGHT = 2f;

		public SharedPathCalculationState PathCalculationState;

		public SharedPathFollowingState PathFollowingState;

		public SharedVector3 PathGoal;

		public SharedVector3 PathStart;

		public SharedAIBehaviorTreeAgentNode agent;

		public SharedPointArray Path;

		public SharedPathNodeArray WayPointsLinkedList;

		public SharedAICaptureInfo CaptureInfo;

		private NavMeshPath _navMeshPath = new NavMeshPath();

		private PathNode _nextWayPoint;

		private AIAgentDataComponentsNode _agent;

		private AICaptureInfo _captureInfo;

		public CheckPathValidConditional()
			: this()
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown


		public override void OnStart()
		{
			_agent = agent.get_Value();
			_captureInfo = CaptureInfo.get_Value();
			SetInitialWayPoint(WayPointsLinkedList.get_Value());
			this.OnStart();
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			TaskStatus val = 2;
			if (PathFollowingState.get_Value() == Simulation.SinglePlayer.BehaviorTree.PathFollowingState.Running)
			{
				return 2;
			}
			if (PathFollowingState.get_Value() == Simulation.SinglePlayer.BehaviorTree.PathFollowingState.Complete)
			{
				_nextWayPoint = _nextWayPoint.NextNode;
			}
			PathGoal.set_Value(GetPathDirection());
			PathStart.set_Value(_agent.aiMovementData.position);
			if (!TryCalculatePath(PathStart.get_Value(), PathGoal.get_Value(), _navMeshPath))
			{
				PathCalculationState.set_Value(Simulation.SinglePlayer.BehaviorTree.PathCalculationState.NoPath);
				return 1;
			}
			PathCalculationState.set_Value(Simulation.SinglePlayer.BehaviorTree.PathCalculationState.PathCalculated);
			PathFollowingState.set_Value(Simulation.SinglePlayer.BehaviorTree.PathFollowingState.Start);
			Path.set_Value(_navMeshPath.get_corners());
			return 2;
		}

		private Vector3 GetPathDirection()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = _nextWayPoint.position;
			if (_captureInfo.Goal != null)
			{
				val = _captureInfo.Goal.Position;
			}
			if (Physics.Raycast(_agent.aiMovementData.rigidBody.get_centerOfMass(), _agent.aiMovementData.forward, _agent.aiMovementData.horizontalRadius * 0.5f))
			{
				Vector3 val2 = Vector3.ProjectOnPlane(_agent.aiMovementData.position, Vector3.get_up());
				Vector3 val3 = val - val2;
				Vector3 val4 = val2 - val;
				Vector3 val5 = Vector3.Cross(val3, _agent.aiMovementData.forward);
				Vector3 val6 = Vector3.Cross(_agent.aiMovementData.forward, val5);
				Vector3 val7 = Vector3.Reflect(val4, val6);
				val = val2 + val7;
			}
			return val;
		}

		private bool TryCalculatePath(Vector3 start, Vector3 goal, NavMeshPath navMeshPath)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (!NavMesh.CalculatePath(start, goal, -1, navMeshPath))
			{
				float radius = 4f;
				FindNearestValidPoint(start, out start, radius);
				FindNearestValidPoint(goal, out goal, radius);
				if (NavMesh.CalculatePath(start, goal, -1, navMeshPath))
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public static bool FindNearestValidPoint(Vector3 position, out Vector3 newPosition, float radius)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			newPosition = position;
			NavMeshHit val = default(NavMeshHit);
			if (NavMesh.SamplePosition(position, ref val, radius, -1))
			{
				newPosition = val.get_position();
				return true;
			}
			return false;
		}

		private void SetInitialWayPoint(PathNode[] wayPoints)
		{
			if (_nextWayPoint == null)
			{
				Random random = new Random(Guid.NewGuid().GetHashCode());
				int num = random.Next(0, wayPoints.Length);
				_nextWayPoint = wayPoints[num];
			}
		}
	}
}
