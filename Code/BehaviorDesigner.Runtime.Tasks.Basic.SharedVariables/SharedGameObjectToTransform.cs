using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Gets the Transform from the GameObject. Returns Success.")]
	public class SharedGameObjectToTransform : Action
	{
		[Tooltip("The GameObject to get the Transform of")]
		public SharedGameObject sharedGameObject;

		[RequiredField]
		[Tooltip("The Transform to set")]
		public SharedTransform sharedTransform;

		public SharedGameObjectToTransform()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (!(sharedGameObject.get_Value() == null))
			{
				sharedTransform.set_Value(sharedGameObject.get_Value().GetComponent<Transform>());
				return 2;
			}
			return 1;
		}

		public override void OnReset()
		{
			sharedGameObject = null;
			sharedTransform = null;
		}
	}
}