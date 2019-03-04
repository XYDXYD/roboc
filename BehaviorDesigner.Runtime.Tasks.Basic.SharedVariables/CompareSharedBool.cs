namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	public class CompareSharedBool : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedBool variable;

		[Tooltip("The variable to compare to")]
		public SharedBool compareTo;

		public CompareSharedBool()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!variable.get_Value().Equals(compareTo.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			variable = false;
			compareTo = false;
		}
	}
}
