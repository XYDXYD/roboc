namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Flips the value of the bool.")]
	public class BoolFlip : Action
	{
		[Tooltip("The bool to flip the value of")]
		public SharedBool boolVariable;

		public BoolFlip()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			boolVariable.set_Value(!boolVariable.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			boolVariable.set_Value(false);
		}
	}
}
