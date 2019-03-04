using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector3
{
	[TaskCategory("Basic/Vector3")]
	[TaskDescription("Stores the forward vector value.")]
	public class GetForwardVector : Action
	{
		[Tooltip("The stored result")]
		[RequiredField]
		public SharedVector3 storeResult;

		public GetForwardVector()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Vector3.get_forward());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			storeResult = Vector3.get_zero();
		}
	}
}
