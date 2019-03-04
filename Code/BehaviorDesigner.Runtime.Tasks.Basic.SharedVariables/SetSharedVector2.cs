using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedVector2 variable to the specified object. Returns Success.")]
	public class SetSharedVector2 : Action
	{
		[Tooltip("The value to set the SharedVector2 to")]
		public SharedVector2 targetValue;

		[RequiredField]
		[Tooltip("The SharedVector2 to set")]
		public SharedVector2 targetVariable;

		public SetSharedVector2()
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
			targetValue = Vector2.get_zero();
			targetVariable = Vector2.get_zero();
		}
	}
}
