namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedObjectList variable to the specified object. Returns Success.")]
	public class SetSharedObjectList : Action
	{
		[Tooltip("The value to set the SharedObjectList to.")]
		public SharedObjectList targetValue;

		[RequiredField]
		[Tooltip("The SharedObjectList to set")]
		public SharedObjectList targetVariable;

		public SetSharedObjectList()
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
