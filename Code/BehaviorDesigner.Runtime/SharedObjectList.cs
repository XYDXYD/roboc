using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedObjectList : SharedVariable<List<Object>>
	{
		public static implicit operator SharedObjectList(List<Object> value)
		{
			SharedObjectList sharedObjectList = new SharedObjectList();
			sharedObjectList.mValue = (_00210)value;
			return sharedObjectList;
		}
	}
}
