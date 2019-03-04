namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedGameObjectList variable to the specified object. Returns Success.")]
	public class SetSharedGameObjectList : Action
	{
		[Tooltip("The value to set the SharedGameObjectList to.")]
		public SharedGameObjectList targetValue;

		[RequiredField]
		[Tooltip("The SharedGameObjectList to set")]
		public SharedGameObjectList targetVariable;

		public SetSharedGameObjectList()
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
