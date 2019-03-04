using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

internal class InputAssignmentButton : MonoBehaviour
{
	public enum InputType
	{
		Keyboard,
		Mouse,
		Joypad
	}

	public Transform listenerParent;

	public InputType inputType;

	public UILabel inputText;

	[NonSerialized]
	public int categoryId;

	[NonSerialized]
	public int buttonId;

	public InputAssignmentButton()
		: this()
	{
	}

	public void OnClick()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (this.get_enabled())
		{
			new SignalChain(listenerParent).Broadcast<InputAssignmentButton>(this);
		}
	}

	public void ResetBlock()
	{
		UIButton[] components = this.GetComponents<UIButton>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].set_isEnabled(true);
			components[i].SetState(0, true);
		}
		this.set_enabled(true);
	}

	public void Disable()
	{
		this.set_enabled(false);
	}

	public void Block()
	{
		UIButton[] components = this.GetComponents<UIButton>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].set_isEnabled(false);
			components[i].SetState(3, true);
		}
		this.set_enabled(false);
	}
}
