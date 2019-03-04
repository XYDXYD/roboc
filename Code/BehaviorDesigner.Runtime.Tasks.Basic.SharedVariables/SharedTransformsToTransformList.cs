using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedTransformList values from the Transforms. Returns Success.")]
	public class SharedTransformsToTransformList : Action
	{
		[Tooltip("The Transforms value")]
		public SharedTransform[] transforms;

		[RequiredField]
		[Tooltip("The SharedTransformList to set")]
		public SharedTransformList storedTransformList;

		public SharedTransformsToTransformList()
			: this()
		{
		}

		public override void OnAwake()
		{
			storedTransformList.set_Value(new List<Transform>());
		}

		public override TaskStatus OnUpdate()
		{
			if (transforms != null && transforms.Length != 0)
			{
				storedTransformList.get_Value().Clear();
				for (int i = 0; i < transforms.Length; i++)
				{
					storedTransformList.get_Value().Add(transforms[i].get_Value());
				}
				return 2;
			}
			return 1;
		}

		public override void OnReset()
		{
			transforms = null;
			storedTransformList = null;
		}
	}
}
