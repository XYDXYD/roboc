using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityString
{
	[TaskCategory("Basic/String")]
	[TaskDescription("Randomly selects a string from the array of strings.")]
	public class GetRandomString : Action
	{
		[Tooltip("The array of strings")]
		public SharedString[] source;

		[Tooltip("The stored result")]
		[RequiredField]
		public SharedString storeResult;

		public GetRandomString()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(source[Random.Range(0, source.Length)].get_Value());
			return 2;
		}

		public override void OnReset()
		{
			source = null;
			storeResult = null;
		}
	}
}
