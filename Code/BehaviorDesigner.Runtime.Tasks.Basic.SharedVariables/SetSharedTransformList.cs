namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedTransformList variable to the specified object. Returns Success.")]
	public class SetSharedTransformList : Action
	{
		[Tooltip("The value to set the SharedTransformList to.")]
		public SharedTransformList targetValue;

		[RequiredField]
		[Tooltip("The SharedTransformList to set")]
		public SharedTransformList targetVariable;

		public SetSharedTransformList()
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
			targetValue = null;
			targetVariable = null;
		}
	}
}
