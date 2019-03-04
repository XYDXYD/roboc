using Simulation;
using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;

internal class PrefabsHolder : MonoBehaviour, IInitialize, IPingPrefabsHolderComponent, IComponent
{
	public GameObject[] prefabs;

	public bool createAtStartup;

	public bool hideOnStartup;

	[Inject]
	internal IGameObjectFactory factory
	{
		private get;
		set;
	}

	public PrefabsHolder()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < prefabs.Length; i++)
		{
			factory.RegisterPrefab(prefabs[i], prefabs[i].get_name(), this.get_transform().get_parent().get_gameObject());
		}
		if (!createAtStartup)
		{
			return;
		}
		for (int j = 0; j < prefabs.Length; j++)
		{
			GameObject val = factory.Build(prefabs[j].get_name());
			this.get_transform().set_localPosition(Vector3.get_zero());
			this.get_transform().set_localScale(Vector3.get_one());
			if (hideOnStartup)
			{
				val.SetActive(false);
			}
		}
	}

	public GameObject GetPingPrefabOfType(PingType type)
	{
		return prefabs[(int)type];
	}
}
