using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityNavMeshAgent
{
	[TaskCategory("Basic/NavMeshAgent")]
	[TaskDescription("Sets the maximum movement speed when following a path. Returns Success.")]
	public class SetSpeed : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The NavMeshAgent speed")]
		public SharedFloat speed;

		private NavMeshAgent navMeshAgent;

		private GameObject prevGameObject;

		public SetSpeed()
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
			navMeshAgent.set_speed(speed.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			speed = 0f;
		}
	}
}
