using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedVector4 variable to the specified object. Returns Success.")]
	public class SetSharedVector4 : Action
	{
		[Tooltip("The value to set the SharedVector4 to")]
		public SharedVector4 targetValue;

		[RequiredField]
		[Tooltip("The SharedVector4 to set")]
		public SharedVector4 targetVariable;

		public SetSharedVector4()
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
			targetValue = Vector4.get_zero();
			targetVariable = Vector4.get_zero();
		}
	}
}
