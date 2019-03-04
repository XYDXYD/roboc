using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	public class CompareSharedColor : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedColor variable;

		[Tooltip("The variable to compare to")]
		public SharedColor compareTo;

		public CompareSharedColor()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			Color value = variable.get_Value();
			return (!((object)value).Equals((object)compareTo.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			variable = Color.get_black();
			compareTo = Color.get_black();
		}
	}
}
