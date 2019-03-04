using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedMaterial : SharedVariable<Material>
	{
		public static implicit operator SharedMaterial(Material value)
		{
			SharedMaterial sharedMaterial = new SharedMaterial();
			sharedMaterial.mValue = (_00210)value;
			return sharedMaterial;
		}
	}
}
