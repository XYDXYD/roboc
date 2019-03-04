using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedColor variable to the specified object. Returns Success.")]
	public class SetSharedColor : Action
	{
		[Tooltip("The value to set the SharedColor to")]
		public SharedColor targetValue;

		[RequiredField]
		[Tooltip("The SharedColor to set")]
		public SharedColor targetVariable;

		public SetSharedColor()
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
			targetValue = Color.get_black();
			targetVariable = Color.get_black();
		}
	}
}
