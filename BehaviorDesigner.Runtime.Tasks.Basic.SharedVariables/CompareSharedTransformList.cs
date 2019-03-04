namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	public class CompareSharedTransformList : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedTransformList variable;

		[Tooltip("The variable to compare to")]
		public SharedTransformList compareTo;

		public CompareSharedTransformList()
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
			if (variable.get_Value().Count == compareTo.get_Value().Count)
			{
				for (int i = 0; i < variable.get_Value().Count; i++)
				{
					if (variable.get_Value()[i] != compareTo.get_Value()[i])
					{
						return 1;
					}
				}
				return 2;
			}
			return 1;
		}

		public override void OnReset()
		{
			variable = null;
			compareTo = null;
		}
	}
}
