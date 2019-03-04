using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

internal class InputAssignmentListBox : MonoBehaviour
{
	public Transform listenerParent;

	public InputAssignmentButton.InputType inputType;

	public UIPopupList listBox;

	[NonSerialized]
	public string initialValue;

	[NonSerialized]
	public int categoryId;

	[NonSerialized]
	public int buttonId;

	public InputAssignmentListBox()
		: this()
	{
	}

	public void OnCurrentSelectionChanged()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (this.get_enabled() && WasValueChanged() && HasValue())
		{
			new SignalChain(listenerParent).Broadcast<InputAssignmentListBox>(this);
		}
	}

	private bool WasValueChanged()
	{
		return initialValue != listBox.get_value();
	}

	private bool HasValue()
	{
		return listBox.get_value() != listBox.items[0];
	}
}
