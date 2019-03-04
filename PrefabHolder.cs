using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

internal class PrefabHolder : MonoBehaviour, IInitialize
{
	public bool createAtStartup;

	public bool hideOnStartup;

	public GameObject prefab;

	[Inject]
	internal IGameObjectFactory factory
	{
		private get;
		set;
	}

	public PrefabHolder()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (prefab == null)
		{
			throw new Exception("Using a prefabholder without gameobject (missing) " + this.get_name());
		}
		string prefabNameWithoutPlatformSuffix = GetPrefabNameWithoutPlatformSuffix();
		factory.RegisterPrefab(prefab, prefabNameWithoutPlatformSuffix, this.get_gameObject());
		if (createAtStartup)
		{
			GameObject val = factory.Build(prefabNameWithoutPlatformSuffix);
			this.get_transform().set_localPosition(Vector3.get_zero());
			this.get_transform().set_localScale(Vector3.get_one());
			if (hideOnStartup)
			{
				val.SetActive(false);
			}
		}
	}

	private string GetPrefabNameWithoutPlatformSuffix()
	{
		string name = prefab.get_name();
		string platformSpecificSuffix = GetPlatformSpecificSuffix();
		int num = name.Length - platformSpecificSuffix.Length;
		if (num > 0 && name.Substring(num).Equals(platformSpecificSuffix))
		{
			return name.Substring(0, num);
		}
		return name;
	}

	private string GetPlatformSpecificSuffix()
	{
		return "_Tencent";
	}
}
