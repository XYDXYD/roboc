using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Finds a GameObject by name. Returns Success.")]
	public class Find : Action
	{
		[Tooltip("The GameObject name to find")]
		public SharedString gameObjectName;

		[Tooltip("The object found by name")]
		[RequiredField]
		public SharedGameObject storeValue;

		public Find()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeValue.set_Value(GameObject.Find(gameObjectName.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			gameObjectName = null;
			storeValue = null;
		}
	}
}
