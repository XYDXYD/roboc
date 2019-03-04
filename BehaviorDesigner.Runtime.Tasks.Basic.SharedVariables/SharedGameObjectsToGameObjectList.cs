using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedGameObjectList values from the GameObjects. Returns Success.")]
	public class SharedGameObjectsToGameObjectList : Action
	{
		[Tooltip("The GameObjects value")]
		public SharedGameObject[] gameObjects;

		[RequiredField]
		[Tooltip("The SharedTransformList to set")]
		public SharedGameObjectList storedGameObjectList;

		public SharedGameObjectsToGameObjectList()
			: this()
		{
		}

		public override void OnAwake()
		{
			storedGameObjectList.set_Value(new List<GameObject>());
		}

		public override TaskStatus OnUpdate()
		{
			if (gameObjects != null && gameObjects.Length != 0)
			{
				storedGameObjectList.get_Value().Clear();
				for (int i = 0; i < gameObjects.Length; i++)
				{
					storedGameObjectList.get_Value().Add(gameObjects[i].get_Value());
				}
				return 2;
			}
			return 1;
		}

		public override void OnReset()
		{
			gameObjects = null;
			storedGameObjectList = null;
		}
	}
}
