namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Performs a comparison between two bools.")]
	public class BoolComparison : Conditional
	{
		[Tooltip("The first bool")]
		public SharedBool bool1;

		[Tooltip("The second bool")]
		public SharedBool bool2;

		public BoolComparison()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (bool1.get_Value() != bool2.get_Value()) ? 1 : 2;
		}

		public override void OnReset()
		{
			bool1.set_Value(false);
			bool2.set_Value(false);
		}
	}
}
