namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	public class CompareSharedTransform : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedTransform variable;

		[Tooltip("The variable to compare to")]
		public SharedTransform compareTo;

		public CompareSharedTransform()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (variable.get_Value() == null && compareTo.get_Value() != null)
			{
				return 1;
			}
			if (variable.get_Value() == null && compareTo.get_Value() == null)
			{
				return 2;
			}
			return (!((object)variable.get_Value()).Equals((object)compareTo.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			variable = null;
			compareTo = null;
		}
	}
}
