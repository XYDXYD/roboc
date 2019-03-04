using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;

internal class MultiPrefabHolder : MonoBehaviour, IInitialize
{
	public GameObject[] prefabs;

	[Inject]
	internal IGameObjectFactory factory
	{
		private get;
		set;
	}

	public MultiPrefabHolder()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		GameObject[] array = prefabs;
		foreach (GameObject val in array)
		{
			factory.RegisterPrefab(val, val.get_name(), null);
		}
	}
}
