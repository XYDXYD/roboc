namespace Simulation
{
	public sealed class EmpMainBeamPool : ObjectPool<EmpMainBeamBehaviour>
	{
		public override void Recycle(EmpMainBeamBehaviour go, int pool)
		{
			base.Recycle(go, pool);
			go.get_gameObject().SetActive(false);
		}

		public override void Recycle(EmpMainBeamBehaviour go, string poolName)
		{
			base.Recycle(go, poolName);
			go.get_gameObject().SetActive(false);
		}
	}
}
