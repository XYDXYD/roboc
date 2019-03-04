using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Simulation
{
	internal class MachineSphereColliderPool : ObjectPool<SphereCollider>
	{
		public const int PREALLOCATED_SPHERE_COLLIDERS = 20;

		[CompilerGenerated]
		private static Func<SphereCollider> _003C_003Ef__mg_0024cache0;

		public MachineSphereColliderPool()
		{
			base.Preallocate(0, 20, (Func<SphereCollider>)MicrobotCollisionSphere.GenerateNewSphereCollider);
		}
	}
}
