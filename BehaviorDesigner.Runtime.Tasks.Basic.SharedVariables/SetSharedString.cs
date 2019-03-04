namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedString variable to the specified object. Returns Success.")]
	public class SetSharedString : Action
	{
		[Tooltip("The value to set the SharedString to")]
		public SharedString targetValue;

		[RequiredField]
		[Tooltip("The SharedString to set")]
		public SharedString targetVariable;

		public SetSharedString()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			targetVariable.set_Value(targetValue.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetValue = string.Empty;
			targetVariable = string.Empty;
		}
	}
}
