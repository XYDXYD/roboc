using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRenderer
{
	[TaskCategory("Basic/Renderer")]
	[TaskDescription("Returns Success if the Renderer is visible, otherwise Failure.")]
	public class IsVisible : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		private Renderer renderer;

		private GameObject prevGameObject;

		public IsVisible()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				renderer = defaultGameObject.GetComponent<Renderer>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (renderer == null)
			{
				Debug.LogWarning((object)"Renderer is null");
				return 1;
			}
			return (!renderer.get_isVisible()) ? 1 : 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
