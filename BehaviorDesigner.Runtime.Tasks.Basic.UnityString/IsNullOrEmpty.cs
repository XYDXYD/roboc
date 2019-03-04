namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityString
{
	[TaskCategory("Basic/String")]
	[TaskDescription("Returns success if the string is null or empty")]
	public class IsNullOrEmpty : Conditional
	{
		[Tooltip("The target string")]
		public SharedString targetString;

		public IsNullOrEmpty()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!string.IsNullOrEmpty(targetString.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			targetString = string.Empty;
		}
	}
}
