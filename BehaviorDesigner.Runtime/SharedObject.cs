using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedObject : SharedVariable<Object>
	{
		public static explicit operator SharedObject(Object value)
		{
			SharedObject sharedObject = new SharedObject();
			sharedObject.mValue = (_00210)value;
			return sharedObject;
		}
	}
}
