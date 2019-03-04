using UnityEngine;

namespace Simulation.BattleArena
{
	internal interface IFusionShieldColliderComponent
	{
		FusionShieldCollider enterRangeShieldCollider
		{
			get;
		}

		FusionShieldCollider shieldCapsuleCollider
		{
			get;
		}

		FusionShieldCollider[] shieldSphereColliders
		{
			get;
		}

		MeshCollider shieldMeshCollider
		{
			get;
		}

		bool machineBlockColliderEnabled
		{
			get;
			set;
		}
	}
}
