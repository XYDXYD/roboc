namespace Simulation
{
	internal sealed class EmpTargetingLocatorPool : ObjectPool<EmpTargetingLocatorMonoBehaviour>
	{
		public override void Recycle(EmpTargetingLocatorMonoBehaviour go, int pool)
		{
			base.Recycle(go, pool);
			go.get_gameObject().SetActive(false);
		}

		public override void Recycle(EmpTargetingLocatorMonoBehaviour go, string poolName)
		{
			base.Recycle(go, poolName);
			go.get_gameObject().SetActive(false);
		}
	}
}
