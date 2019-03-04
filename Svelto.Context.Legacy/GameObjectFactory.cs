using Svelto.DataStructures;
using Svelto.Factories;
using System.Collections.Generic;
using UnityEngine;

namespace Svelto.Context.Legacy
{
	public class GameObjectFactory : IGameObjectFactory
	{
		private readonly Dictionary<string, GameObject[]> _prefabs;

		private readonly WeakReference<IUnityContextHierarchyChangedListener> _unityContext;

		public GameObjectFactory(IUnityContextHierarchyChangedListener root)
		{
			_unityContext = new WeakReference<IUnityContextHierarchyChangedListener>(root);
			_prefabs = new Dictionary<string, GameObject[]>();
		}

		public GameObject Build(string prefabName)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = Build(_prefabs[prefabName][0]);
			GameObject val2 = _prefabs[prefabName][1];
			if (val2 == null)
			{
				return val;
			}
			Transform transform = val.get_transform();
			Vector3 localScale = transform.get_localScale();
			Quaternion localRotation = transform.get_localRotation();
			Vector3 localPosition = transform.get_localPosition();
			val2.SetActive(true);
			transform.set_parent(val2.get_transform());
			transform.set_localPosition(localPosition);
			transform.set_localRotation(localRotation);
			transform.set_localScale(localScale);
			return val;
		}

		public GameObject Build(GameObject prefab)
		{
			GameObject val = Object.Instantiate<GameObject>(prefab);
			MonoBehaviour[] componentsInChildren = val.GetComponentsInChildren<MonoBehaviour>(true);
			foreach (MonoBehaviour val2 in componentsInChildren)
			{
				if (!(val2 == null))
				{
					GameObject gameObject = val2.get_gameObject();
					_unityContext.get_Target().OnMonobehaviourAdded(val2);
					if (gameObject.GetComponent<NotifyComponentsRemoved>() == null)
					{
						gameObject.AddComponent<NotifyComponentsRemoved>().unityContext = _unityContext;
					}
				}
			}
			return val;
		}

		public void RegisterPrefab(GameObject prefab, string prefabName, GameObject parent = null)
		{
			GameObject[] value = (GameObject[])new GameObject[2]
			{
				prefab,
				parent
			};
			_prefabs.Add(prefabName, value);
		}
	}
}
