namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityString
{
	[TaskCategory("Basic/String")]
	[TaskDescription("Sets the variable string to the value string.")]
	public class SetString : Action
	{
		[Tooltip("The target string")]
		[RequiredField]
		public SharedString variable;

		[Tooltip("The value string")]
		public SharedString value;

		public SetString()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			variable.set_Value(value.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			variable = string.Empty;
			value = string.Empty;
		}
	}
}
