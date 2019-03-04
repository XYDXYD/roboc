namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityString
{
	[TaskCategory("Basic/String")]
	[TaskDescription("Creates a string from multiple other strings.")]
	public class BuildString : Action
	{
		[Tooltip("The array of strings")]
		public SharedString[] source;

		[Tooltip("The stored result")]
		[RequiredField]
		public SharedString storeResult;

		public BuildString()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			for (int i = 0; i < source.Length; i++)
			{
				SharedString sharedString = storeResult;
				sharedString.set_Value(sharedString.get_Value() + source[i]);
			}
			return 2;
		}

		public override void OnReset()
		{
			source = null;
			storeResult = null;
		}
	}
}
