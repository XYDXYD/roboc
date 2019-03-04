using Fabric;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class PassThroughSoundPlayer : MonoBehaviour
{
	private Dictionary<GameObject, GameObject> _robotSoundObjectDictionary = new Dictionary<GameObject, GameObject>();

	private List<GameObject> _robotsToRemove = new List<GameObject>();

	[Inject]
	internal GameObjectPool gameObjectPool
	{
		private get;
		set;
	}

	public PassThroughSoundPlayer()
		: this()
	{
	}

	private void Start()
	{
		gameObjectPool.Preallocate("soundObjects", 2, (Func<GameObject>)CreateSoundObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		GameObject gameObject = other.get_attachedRigidbody().get_gameObject();
		if (!_robotSoundObjectDictionary.ContainsKey(gameObject))
		{
			GameObject val = gameObjectPool.Use("soundObjects", (Func<GameObject>)CreateSoundObject);
			_robotSoundObjectDictionary.Add(gameObject, val);
			EventManager.get_Instance().PostEvent("DiscShield_Through", 0, val);
		}
	}

	private void Update()
	{
		Dictionary<GameObject, GameObject>.Enumerator enumerator = _robotSoundObjectDictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameObject value = enumerator.Current.Value;
			GameObject key = enumerator.Current.Key;
			if (!EventManager.get_Instance().IsEventActive("DiscShield_Through", value))
			{
				_robotsToRemove.Add(key);
			}
		}
		for (int i = 0; i < _robotsToRemove.Count; i++)
		{
			gameObjectPool.Recycle(_robotSoundObjectDictionary[_robotsToRemove[i]], "soundObjects");
			_robotSoundObjectDictionary.Remove(_robotsToRemove[i]);
		}
		_robotsToRemove.Clear();
	}

	private GameObject CreateSoundObject()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Object.Instantiate<GameObject>(new GameObject("sound_obj"));
		val.get_transform().set_parent(this.get_transform());
		val.get_transform().set_localPosition(Vector3.get_zero());
		val.get_transform().set_localRotation(Quaternion.get_identity());
		val.get_transform().set_localScale(Vector3.get_one());
		return val;
	}
}
