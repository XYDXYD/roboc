using UnityEngine;

internal class ParticleSystemUpdateBehaviour : MonoBehaviour
{
	public ParticleSystemUpdateItem[] particlesToUpdate;

	private ParticleSystemUpdaterObjectPool _pool;

	private string _poolName;

	public ParticleSystemUpdateBehaviour()
		: this()
	{
	}

	public void SetParticleEmissionPercent(float percent)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < particlesToUpdate.Length; i++)
		{
			ParticleSystemUpdateItem particleSystemUpdateItem = particlesToUpdate[i];
			float num = Mathf.Lerp(particleSystemUpdateItem.initialValue, particleSystemUpdateItem.finalValue, percent);
			switch (particleSystemUpdateItem.type)
			{
			case typeUpdater.Size:
				particleSystemUpdateItem.particleSystem.set_startSize(num);
				break;
			case typeUpdater.Alpha:
			{
				Color startColor = particleSystemUpdateItem.particleSystem.get_startColor();
				startColor.a = num;
				particleSystemUpdateItem.particleSystem.set_startColor(startColor);
				break;
			}
			case typeUpdater.MaxParticles:
				particleSystemUpdateItem.particleSystem.set_maxParticles(Mathf.CeilToInt(num));
				break;
			}
		}
	}

	public void SetPool(ParticleSystemUpdaterObjectPool pool, string poolName)
	{
		_pool = pool;
		_poolName = poolName;
	}

	private void OnDisable()
	{
		if (_pool != null)
		{
			_pool.Recycle(this, _poolName);
		}
	}
}
