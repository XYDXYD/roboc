using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRenderer
{
	[TaskCategory("Basic/Renderer")]
	[TaskDescription("Sets the material on the Renderer.")]
	public class SetMaterial : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The material to set")]
		public SharedMaterial material;

		private Renderer renderer;

		private GameObject prevGameObject;

		public SetMaterial()
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
			renderer.set_material(material.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			material = null;
		}
	}
}
