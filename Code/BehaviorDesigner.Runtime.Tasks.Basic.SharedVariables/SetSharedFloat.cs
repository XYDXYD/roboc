namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedFloat variable to the specified object. Returns Success.")]
	public class SetSharedFloat : Action
	{
		[Tooltip("The value to set the SharedFloat to")]
		public SharedFloat targetValue;

		[RequiredField]
		[Tooltip("The SharedFloat to set")]
		public SharedFloat targetVariable;

		public SetSharedFloat()
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
			targetValue = 0f;
			targetVariable = 0f;
		}
	}
}
