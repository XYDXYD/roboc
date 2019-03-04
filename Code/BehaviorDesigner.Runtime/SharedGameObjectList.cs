using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedGameObjectList : SharedVariable<List<GameObject>>
	{
		public static implicit operator SharedGameObjectList(List<GameObject> value)
		{
			SharedGameObjectList sharedGameObjectList = new SharedGameObjectList();
			sharedGameObjectList.mValue = (_00210)value;
			return sharedGameObjectList;
		}
	}
}
