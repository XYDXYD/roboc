using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedGameObject : SharedVariable<GameObject>
	{
		public static implicit operator SharedGameObject(GameObject value)
		{
			SharedGameObject sharedGameObject = new SharedGameObject();
			sharedGameObject.mValue = (_00210)value;
			return sharedGameObject;
		}
	}
}
