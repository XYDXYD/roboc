using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Returns Success if the layermasks match, otherwise Failure.")]
	public class CompareLayerMask : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The layermask to compare against")]
		public LayerMask layermask;

		public CompareLayerMask()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return ((this.GetDefaultGameObject(targetGameObject.get_Value()).get_layer() & layermask.get_value()) == 0) ? 1 : 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
