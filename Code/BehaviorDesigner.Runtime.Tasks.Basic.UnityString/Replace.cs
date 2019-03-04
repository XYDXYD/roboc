namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityString
{
	[TaskCategory("Basic/String")]
	[TaskDescription("Replaces a string with the new string")]
	public class Replace : Action
	{
		[Tooltip("The target string")]
		public SharedString targetString;

		[Tooltip("The old replace")]
		public SharedString oldString;

		[Tooltip("The new string")]
		public SharedString newString;

		[Tooltip("The stored result")]
		[RequiredField]
		public SharedString storeResult;

		public Replace()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(targetString.get_Value().Replace(oldString.get_Value(), newString.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			targetString = string.Empty;
			oldString = string.Empty;
			newString = string.Empty;
			storeResult = string.Empty;
		}
	}
}
