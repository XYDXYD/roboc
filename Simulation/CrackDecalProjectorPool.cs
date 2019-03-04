namespace Simulation
{
	internal sealed class CrackDecalProjectorPool : ObjectPool<CrackDecalProjectorAutoRecycleBehaviour>
	{
		public override void Recycle(CrackDecalProjectorAutoRecycleBehaviour go, int pool)
		{
			base.Recycle(go, pool);
			go.get_gameObject().SetActive(false);
		}

		public override void Recycle(CrackDecalProjectorAutoRecycleBehaviour go, string poolName)
		{
			base.Recycle(go, poolName);
			go.get_gameObject().SetActive(false);
		}
	}
}
