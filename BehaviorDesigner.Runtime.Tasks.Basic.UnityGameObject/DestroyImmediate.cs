using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Destorys the specified GameObject immediately. Returns Success.")]
	public class DestroyImmediate : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		public DestroyImmediate()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			Object.DestroyImmediate(defaultGameObject);
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
