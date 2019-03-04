using UnityEngine;

internal class AutoRecycleParticleSystemBehaviour : AutoRecycleBehaviour<ParticleSystem>
{
	private new void OnDisable()
	{
		if (_id != -1)
		{
			_pool.Recycle(_object, _id);
		}
		else
		{
			_pool.Recycle(_object, _poolName);
		}
		_object.get_gameObject().SetActive(false);
	}
}
