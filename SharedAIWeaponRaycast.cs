using BehaviorDesigner.Runtime;
using System;

[Serializable]
public class SharedAIWeaponRaycast : SharedVariable<AIWeaponRaycast>
{
	public static implicit operator SharedAIWeaponRaycast(AIWeaponRaycast value)
	{
		SharedAIWeaponRaycast sharedAIWeaponRaycast = new SharedAIWeaponRaycast();
		sharedAIWeaponRaycast.set_Value(value);
		return sharedAIWeaponRaycast;
	}
}
