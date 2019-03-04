using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	public class CompareSharedVector4 : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedVector4 variable;

		[Tooltip("The variable to compare to")]
		public SharedVector4 compareTo;

		public CompareSharedVector4()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			Vector4 value = variable.get_Value();
			return (!((object)value).Equals((object)compareTo.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			variable = Vector4.get_zero();
			compareTo = Vector4.get_zero();
		}
	}
}
