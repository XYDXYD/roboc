namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedBool variable to the specified object. Returns Success.")]
	public class SetSharedBool : Action
	{
		[Tooltip("The value to set the SharedBool to")]
		public SharedBool targetValue;

		[RequiredField]
		[Tooltip("The SharedBool to set")]
		public SharedBool targetVariable;

		public SetSharedBool()
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
			targetValue = false;
			targetVariable = false;
		}
	}
}
