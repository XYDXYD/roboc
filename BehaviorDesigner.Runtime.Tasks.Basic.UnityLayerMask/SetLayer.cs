using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityLayerMask
{
	[TaskCategory("Basic/LayerMask")]
	[TaskDescription("Sets the layer of a GameObject.")]
	public class SetLayer : Action
	{
		[Tooltip("The GameObject to set the layer of")]
		public SharedGameObject targetGameObject;

		[Tooltip("The name of the layer to set")]
		public SharedString layerName = "Default";

		public SetLayer()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			defaultGameObject.set_layer(LayerMask.NameToLayer(layerName.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			layerName = "Default";
		}
	}
}
