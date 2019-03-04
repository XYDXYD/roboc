using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityNavMeshAgent
{
	[TaskCategory("Basic/NavMeshAgent")]
	[TaskDescription("Sets the maximum turning speed in (deg/s) while following a path. Returns Success.")]
	public class SetAngularSpeed : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The NavMeshAgent angular speed")]
		public SharedFloat angularSpeed;

		private NavMeshAgent navMeshAgent;

		private GameObject prevGameObject;

		public SetAngularSpeed()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				navMeshAgent = defaultGameObject.GetComponent<NavMeshAgent>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (navMeshAgent == null)
			{
				Debug.LogWarning((object)"NavMeshAgent is null");
				return 1;
			}
			navMeshAgent.set_angularSpeed(angularSpeed.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			angularSpeed = 0f;
		}
	}
}
