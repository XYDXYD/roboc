namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityString
{
	[TaskCategory("Basic/String")]
	[TaskDescription("Stores the length of the string")]
	public class GetLength : Action
	{
		[Tooltip("The target string")]
		public SharedString targetString;

		[Tooltip("The stored result")]
		[RequiredField]
		public SharedInt storeResult;

		public GetLength()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(targetString.get_Value().Length);
			return 2;
		}

		public override void OnReset()
		{
			targetString = string.Empty;
			storeResult = 0;
		}
	}
}
