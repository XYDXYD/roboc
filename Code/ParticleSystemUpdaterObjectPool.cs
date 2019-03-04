using UnityEngine;

internal class ParticleSystemUpdaterObjectPool : ObjectPool<ParticleSystemUpdateBehaviour>
{
	public ParticleSystemUpdateBehaviour CreateGameObjectFromPrefab(GameObject original)
	{
		GameObject val = Object.Instantiate<GameObject>(original);
		val.set_name(original.get_name());
		return val.GetComponent<ParticleSystemUpdateBehaviour>();
	}
}
