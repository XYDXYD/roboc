using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedQuaternion variable to the specified object. Returns Success.")]
	public class SetSharedQuaternion : Action
	{
		[Tooltip("The value to set the SharedQuaternion to")]
		public SharedQuaternion targetValue;

		[RequiredField]
		[Tooltip("The SharedQuaternion to set")]
		public SharedQuaternion targetVariable;

		public SetSharedQuaternion()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			targetVariable.set_Value(targetValue.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			targetValue = Quaternion.get_identity();
			targetVariable = Quaternion.get_identity();
		}
	}
}
