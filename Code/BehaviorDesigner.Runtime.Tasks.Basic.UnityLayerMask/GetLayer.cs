using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityLayerMask
{
	[TaskCategory("Basic/LayerMask")]
	[TaskDescription("Gets the layer of a GameObject.")]
	public class GetLayer : Action
	{
		[Tooltip("The GameObject to set the layer of")]
		public SharedGameObject targetGameObject;

		[Tooltip("The name of the layer to get")]
		[RequiredField]
		public SharedString storeResult;

		public GetLayer()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			storeResult.set_Value(LayerMask.LayerToName(defaultGameObject.get_layer()));
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeResult = string.Empty;
		}
	}
}
