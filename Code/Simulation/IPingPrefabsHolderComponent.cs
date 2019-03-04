using Svelto.ES.Legacy;
using UnityEngine;

namespace Simulation
{
	internal interface IPingPrefabsHolderComponent : IComponent
	{
		GameObject GetPingPrefabOfType(PingType type);
	}
}
