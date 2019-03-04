using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Simulation
{
	internal class MachineClusterPool : ObjectPool<BoxCollider>
	{
		[CompilerGenerated]
		private static Func<BoxCollider> _003C_003Ef__mg_0024cache0;

		public MachineClusterPool()
		{
			base.Preallocate(0, 1024, (Func<BoxCollider>)MachineCluster.GenerateNewCollider);
		}
	}
}
