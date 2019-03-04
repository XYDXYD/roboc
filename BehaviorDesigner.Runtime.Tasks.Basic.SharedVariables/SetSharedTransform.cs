namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedTransform variable to the specified object. Returns Success.")]
	public class SetSharedTransform : Action
	{
		[Tooltip("The value to set the SharedTransform to. If null the variable will be set to the current Transform")]
		public SharedTransform targetValue;

		[RequiredField]
		[Tooltip("The SharedTransform to set")]
		public SharedTransform targetVariable;

		public SetSharedTransform()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			targetVariable.set_Value((!(targetValue.get_Value() != null)) ? base.transform : targetValue.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetValue = null;
			targetVariable = null;
		}
	}
}
