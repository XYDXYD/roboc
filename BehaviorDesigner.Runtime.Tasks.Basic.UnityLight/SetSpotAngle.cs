using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityLight
{
	[TaskCategory("Basic/Light")]
	[TaskDescription("Sets the spot angle of the light.")]
	public class SetSpotAngle : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The spot angle to set")]
		public SharedFloat spotAngle;

		private Light light;

		private GameObject prevGameObject;

		public SetSpotAngle()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				light = defaultGameObject.GetComponent<Light>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (light == null)
			{
				Debug.LogWarning((object)"Light is null");
				return 1;
			}
			light.set_spotAngle(spotAngle.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			spotAngle = 0f;
		}
	}
}
