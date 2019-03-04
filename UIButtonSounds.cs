using Fabric;
using UnityEngine;

internal sealed class UIButtonSounds : MonoBehaviour
{
	[SerializeField]
	private bool _playOnHover = true;

	[SerializeField]
	private bool _playOnClick = true;

	[SerializeField]
	private AudioFabricGameEvents onClickSound = AudioFabricGameEvents.UIButtonSelect;

	[SerializeField]
	private AudioFabricGameEvents onHoverSound = AudioFabricGameEvents.UIButtonHover;

	private bool _isMouseOver;

	internal bool playOnHover
	{
		get
		{
			return _playOnHover;
		}
		set
		{
			_playOnHover = value;
		}
	}

	internal bool playOnClick
	{
		get
		{
			return _playOnClick;
		}
		set
		{
			_playOnClick = value;
		}
	}

	public UIButtonSounds()
		: this()
	{
	}

	internal void OnHover(bool over)
	{
		if (!over || !_isMouseOver)
		{
			if (_playOnHover && over)
			{
				_isMouseOver = true;
				string text = AudioFabricEvent.Name(onHoverSound);
				EventManager.get_Instance().PostEvent(text);
			}
			else
			{
				_isMouseOver = false;
			}
		}
	}

	internal void OnClick()
	{
		if (_playOnClick)
		{
			string text = AudioFabricEvent.Name(onClickSound);
			EventManager.get_Instance().PostEvent(text);
		}
	}

	internal void OnMouseHover()
	{
		if (_playOnHover)
		{
			string text = AudioFabricEvent.Name(AudioFabricGameEvents.UI_Dropdown_Close);
			EventManager.get_Instance().PostEvent(text);
		}
	}
}
