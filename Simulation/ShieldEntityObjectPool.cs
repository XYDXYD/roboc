using UnityEngine;

namespace Simulation
{
	internal sealed class ShieldEntityObjectPool : ObjectPool<ShieldEntity>
	{
		public override void Recycle(ShieldEntity go, int pool)
		{
			base.Recycle(go, pool);
			go.get_gameObject().SetActive(false);
		}

		public override void Recycle(ShieldEntity go, string poolName)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			base.Recycle(go, poolName);
			go.get_transform().set_rotation(Quaternion.Euler(Vector3.get_zero()));
			go.get_gameObject().SetActive(false);
		}
	}
}
