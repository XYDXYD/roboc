namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Sets a float value")]
	public class SetFloat : Action
	{
		[Tooltip("The float value to set")]
		public SharedFloat floatValue;

		[Tooltip("The variable to store the result")]
		public SharedFloat storeResult;

		public SetFloat()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(floatValue.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			floatValue.set_Value(0f);
			storeResult.set_Value(0f);
		}
	}
}
