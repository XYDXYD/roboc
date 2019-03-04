namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Sets an int value")]
	public class SetInt : Action
	{
		[Tooltip("The int value to set")]
		public SharedInt intValue;

		[Tooltip("The variable to store the result")]
		public SharedInt storeResult;

		public SetInt()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(intValue.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			intValue.set_Value(0);
			storeResult.set_Value(0);
		}
	}
}
