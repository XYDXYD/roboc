using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
	[TaskCategory("Basic/Input")]
	[TaskDescription("Stores the acceleration value.")]
	public class GetAcceleration : Action
	{
		[RequiredField]
		[Tooltip("The stored result")]
		public SharedVector3 storeResult;

		public GetAcceleration()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Input.get_acceleration());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			storeResult = Vector3.get_zero();
		}
	}
}
