using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Destorys the specified GameObject. Returns Success.")]
	public class Destroy : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Time to destroy the GameObject in")]
		public float time;

		public Destroy()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (time == 0f)
			{
				Object.Destroy(defaultGameObject);
			}
			else
			{
				Object.Destroy(defaultGameObject, time);
			}
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			time = 0f;
		}
	}
}
