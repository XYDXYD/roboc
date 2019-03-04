using Svelto.Factories;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class PlatoonPlayerList : MonoBehaviour
{
	public PlatoonPlayerListItem playerListElement;

	public Vector3 startPos;

	public Vector3 offset;

	private List<PlatoonPlayerListItem> _playerList = new List<PlatoonPlayerListItem>();

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	public PlatoonPlayerList()
		: this()
	{
	}

	public void Start()
	{
		playerListElement.get_gameObject().SetActive(false);
	}

	internal void ClearList()
	{
		foreach (PlatoonPlayerListItem player in _playerList)
		{
			Object.Destroy(player.get_gameObject());
		}
		_playerList.Clear();
	}

	internal void AddPlayer(string name, PlatoonMember.MemberStatus status, bool isLeader, Dictionary<string, string> actions, Action<string, string> onSelectionChangeCallback)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = gameObjectFactory.Build(playerListElement.get_gameObject());
		val.get_transform().set_parent(playerListElement.get_transform().get_parent());
		val.get_transform().set_localScale(Vector3.get_one());
		val.get_transform().set_localPosition(startPos + (float)_playerList.Count * offset);
		val.SetActive(true);
		PlatoonPlayerListItem component = val.GetComponent<PlatoonPlayerListItem>();
		component.SetData(name, status, isLeader, actions, onSelectionChangeCallback);
		_playerList.Add(component);
	}
}
