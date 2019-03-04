namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityString
{
	[TaskCategory("Basic/String")]
	[TaskDescription("Stores a substring of the target string")]
	public class GetSubstring : Action
	{
		[Tooltip("The target string")]
		public SharedString targetString;

		[Tooltip("The start substring index")]
		public SharedInt startIndex = 0;

		[Tooltip("The length of the substring. Don't use if -1")]
		public SharedInt length = -1;

		[Tooltip("The stored result")]
		[RequiredField]
		public SharedString storeResult;

		public GetSubstring()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (length.get_Value() != -1)
			{
				storeResult.set_Value(targetString.get_Value().Substring(startIndex.get_Value(), length.get_Value()));
			}
			else
			{
				storeResult.set_Value(targetString.get_Value().Substring(startIndex.get_Value()));
			}
			return 2;
		}

		public override void OnReset()
		{
			targetString = string.Empty;
			startIndex = 0;
			length = -1;
			storeResult = string.Empty;
		}
	}
}
