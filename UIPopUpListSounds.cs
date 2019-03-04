using Fabric;
using System;
using UnityEngine;

internal sealed class UIPopUpListSounds : MonoBehaviour
{
	public bool playOnHover = true;

	public AudioFabricGameEvents onPopUpListOpenSound = AudioFabricGameEvents.UIButtonSelect;

	public AudioFabricGameEvents onPopUpListCloseSound = AudioFabricGameEvents.UIButtonSelect;

	public AudioFabricGameEvents onHoverSound = AudioFabricGameEvents.UIButtonHover;

	private bool isMouseOver;

	private UIPopListWithEvents _popUpList;

	public UIPopUpListSounds()
		: this()
	{
	}

	private unsafe void Awake()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		_popUpList = this.GetComponent<UIPopListWithEvents>();
		EventDelegate.Add(_popUpList.onListOpen, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		EventDelegate.Add(_popUpList.onListClose, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	private void OnListClose()
	{
	}

	private void OnListOpen()
	{
	}

	public void OnHover(bool over)
	{
		if (!over || !isMouseOver)
		{
			if (playOnHover && over)
			{
				isMouseOver = true;
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(onHoverSound));
			}
			else
			{
				isMouseOver = false;
			}
		}
	}

	public void OnMouseHover()
	{
		if (playOnHover)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UI_Dropdown_Close));
		}
	}
}
