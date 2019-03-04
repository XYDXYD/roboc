using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedVector3 variable to the specified object. Returns Success.")]
	public class SetSharedVector3 : Action
	{
		[Tooltip("The value to set the SharedVector3 to")]
		public SharedVector3 targetValue;

		[RequiredField]
		[Tooltip("The SharedVector3 to set")]
		public SharedVector3 targetVariable;

		public SetSharedVector3()
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
			targetValue = Vector3.get_zero();
			targetVariable = Vector3.get_zero();
		}
	}
}
