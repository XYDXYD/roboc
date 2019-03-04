namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedGameObject variable to the specified object. Returns Success.")]
	public class SetSharedGameObject : Action
	{
		[Tooltip("The value to set the SharedGameObject to. If null the variable will be set to the current GameObject")]
		public SharedGameObject targetValue;

		[RequiredField]
		[Tooltip("The SharedGameObject to set")]
		public SharedGameObject targetVariable;

		[Tooltip("Can the target value be null?")]
		public SharedBool valueCanBeNull;

		public SetSharedGameObject()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			targetVariable.set_Value((!(targetValue.get_Value() != null) && !valueCanBeNull.get_Value()) ? base.gameObject : targetValue.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			valueCanBeNull = false;
			targetValue = null;
			targetVariable = null;
		}
	}
}
