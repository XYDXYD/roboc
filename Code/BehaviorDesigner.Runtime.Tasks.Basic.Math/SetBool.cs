namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Sets a bool value")]
	public class SetBool : Action
	{
		[Tooltip("The bool value to set")]
		public SharedBool boolValue;

		[Tooltip("The variable to store the result")]
		public SharedBool storeResult;

		public SetBool()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(boolValue.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			boolValue.set_Value(false);
			storeResult.set_Value(false);
		}
	}
}
