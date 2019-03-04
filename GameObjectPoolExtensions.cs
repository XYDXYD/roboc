using Svelto.IoC;
using UnityEngine;

public static class GameObjectPoolExtensions
{
	public static T AddRecycleOnDisableForParticles<T, K>(this IObjectPool<T> pool, GameObject prefab, bool isPrefab = true, bool startDisabled = true) where T : Component where K : AutoRecycleBehaviour<T>
	{
		return pool.AddRecycleOnDisableForComponent<T, K>(prefab, isPrefab, startDisabled);
	}

	public static GameObject AddRecycleOnDisableForParticlesGO(this GameObjectPool pool, GameObject prefab, bool isPrefab = true, bool startDisabled = true)
	{
		return pool.AddRecycleOnDisableForGameObject(prefab, isPrefab, startDisabled);
	}

	public static GameObject AddRecycleOnDisableForLoopParticles(this GameObjectPool pool, GameObject prefab, float time, bool isPrefab = true, bool startDisabled = true)
	{
		GameObject val = prefab;
		if (isPrefab)
		{
			val = GameObjectPool.CreateGameObjectFromPrefab(prefab);
		}
		if (startDisabled)
		{
			val.SetActive(false);
		}
		DisableAfterTime disableAfterTime = val.AddComponent<DisableAfterTime>();
		disableAfterTime.life = time;
		AutoRecycleBehaviour autoRecycleBehaviour = val.AddComponent<AutoRecycleBehaviour>();
		autoRecycleBehaviour.SetPool(pool, val.get_name());
		return val;
	}

	public static T AddRecycleOnDisableForComponent<T, K>(this IObjectPool<T> pool, GameObject prefab, bool isPrefab = true, bool startDisabled = true) where K : AutoRecycleBehaviour<T>
	{
		GameObject val = prefab;
		if (isPrefab)
		{
			val = GameObjectPool.CreateGameObjectFromPrefab(prefab);
		}
		if (startDisabled)
		{
			val.SetActive(false);
		}
		T component = val.GetComponent<T>();
		K val2 = val.AddComponent<K>();
		val2.SetPool(pool, val.get_name());
		val2.SetRecycledComponent(component);
		return component;
	}

	public static GameObject AddGameObjectWithoutRecycle(this GameObjectPool pool, GameObject prefab, bool isPrefab = true, bool startDisabled = true)
	{
		GameObject val = prefab;
		if (isPrefab)
		{
			val = GameObjectPool.CreateGameObjectFromPrefab(prefab);
		}
		if (startDisabled)
		{
			val.SetActive(false);
		}
		return val;
	}

	public static GameObject AddGameObjectWithInjectionInto<T>(this GameObjectPool pool, GameObject prefab, IContainer container, bool isPrefab = true, bool startDisabled = true)
	{
		GameObject val = prefab;
		if (isPrefab)
		{
			val = GameObjectPool.CreateGameObjectFromPrefab(prefab);
		}
		if (startDisabled)
		{
			val.SetActive(false);
		}
		T component = val.GetComponent<T>();
		container.Inject<T>(component);
		return val;
	}

	public static GameObject AddRecycleOnDisableForGameObject(this GameObjectPool pool, GameObject prefab, bool isPrefab = true, bool startDisabled = true)
	{
		GameObject val = prefab;
		if (isPrefab)
		{
			val = GameObjectPool.CreateGameObjectFromPrefab(prefab);
		}
		if (startDisabled)
		{
			val.SetActive(false);
		}
		AutoRecycleBehaviour autoRecycleBehaviour = val.AddComponent<AutoRecycleBehaviour>();
		autoRecycleBehaviour.SetPool(pool, val.get_name());
		return val;
	}

	public static GameObject AddRecycleOnDisableForAudio(this GameObjectPool pool)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		GameObject val = new GameObject("AudioPosition");
		DisableAfterTime disableAfterTime = val.AddComponent<DisableAfterTime>();
		disableAfterTime.life = 2f;
		AutoRecycleBehaviour autoRecycleBehaviour = val.AddComponent<AutoRecycleBehaviour>();
		autoRecycleBehaviour.SetPool(pool, 1);
		return val;
	}
}
