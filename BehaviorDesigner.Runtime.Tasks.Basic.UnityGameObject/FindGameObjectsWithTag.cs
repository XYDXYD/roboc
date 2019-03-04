using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Finds a GameObject by tag. Returns Success.")]
	public class FindGameObjectsWithTag : Action
	{
		[Tooltip("The tag of the GameObject to find")]
		public SharedString tag;

		[Tooltip("The objects found by name")]
		[RequiredField]
		public SharedGameObjectList storeValue;

		public FindGameObjectsWithTag()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(tag.get_Value());
			for (int i = 0; i < array.Length; i++)
			{
				storeValue.get_Value().Add(array[i]);
			}
			return 2;
		}

		public override void OnReset()
		{
			tag.set_Value((string)null);
			storeValue.set_Value((List<GameObject>)null);
		}
	}
}
