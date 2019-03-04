using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityLight
{
	[TaskCategory("Basic/Light")]
	[TaskDescription("Sets the light's cookie size.")]
	public class SetCookieSize : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The size to set")]
		public SharedFloat cookieSize;

		private Light light;

		private GameObject prevGameObject;

		public SetCookieSize()
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
			light.set_cookieSize(cookieSize.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			cookieSize = 0f;
		}
	}
}
