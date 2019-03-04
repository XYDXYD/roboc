using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Finds a GameObject by tag. Returns Success.")]
	public class FindWithTag : Action
	{
		[Tooltip("The tag of the GameObject to find")]
		public SharedString tag;

		[Tooltip("Should a random GameObject be found?")]
		public SharedBool random;

		[Tooltip("The object found by name")]
		[RequiredField]
		public SharedGameObject storeValue;

		public FindWithTag()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (random.get_Value())
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag(tag.get_Value());
				storeValue.set_Value(array[Random.Range(0, array.Length - 1)]);
			}
			else
			{
				storeValue.set_Value(GameObject.FindWithTag(tag.get_Value()));
			}
			return 2;
		}

		public override void OnReset()
		{
			tag.set_Value((string)null);
			storeValue.set_Value(null);
		}
	}
}
