using Svelto.Factories;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class FriendPlayerList : MonoBehaviour
{
	public FriendPlayerListItem playerListElement;

	public UISliderPanelMover friendPanelMover;

	public UISlider friendSlider;

	public Vector3 startPos;

	public Vector3 offset;

	private Dictionary<string, FriendPlayerListItem> _playerList = new Dictionary<string, FriendPlayerListItem>();

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	public FriendPlayerList()
		: this()
	{
	}

	public void Start()
	{
		playerListElement.get_gameObject().SetActive(false);
	}

	internal void ClearList()
	{
		foreach (FriendPlayerListItem value in _playerList.Values)
		{
			Object.Destroy(value.get_gameObject());
		}
		_playerList.Clear();
	}

	internal void AddPlayer(string name, bool isOnline, FriendInviteStatus status, Dictionary<string, string> actions, Action<string, string> callback)
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
		FriendPlayerListItem component = val.GetComponent<FriendPlayerListItem>();
		component.SetData(name, isOnline, status, actions, callback);
		_playerList.Add(name, component);
	}

	internal void UpdateMoverSize()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (_playerList != null)
		{
			float num = (float)_playerList.Count * Mathf.Abs(offset.y);
			friendPanelMover.panelSize = new Vector2(friendPanelMover.panelSize.x, num);
			friendSlider.ForceUpdate();
		}
	}

	internal void UpdateCommands(string friendName, Dictionary<string, string> commands)
	{
		_playerList[friendName].SetCommands(commands);
	}
}
