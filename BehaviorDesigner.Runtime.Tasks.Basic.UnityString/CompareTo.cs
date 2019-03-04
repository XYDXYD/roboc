namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityString
{
	[TaskCategory("Basic/String")]
	[TaskDescription("Compares the first string to the second string. Returns an int which indicates whether the first string precedes, matches, or follows the second string.")]
	public class CompareTo : Action
	{
		[Tooltip("The string to compare")]
		public SharedString firstString;

		[Tooltip("The string to compare to")]
		public SharedString secondString;

		[Tooltip("The stored result")]
		[RequiredField]
		public SharedInt storeResult;

		public CompareTo()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(firstString.get_Value().CompareTo(secondString.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			firstString = string.Empty;
			secondString = string.Empty;
			storeResult = 0;
		}
	}
}
