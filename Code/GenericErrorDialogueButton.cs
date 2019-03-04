using System;
using UnityEngine;

internal sealed class GenericErrorDialogueButton : MonoBehaviour
{
	public Action onClicked
	{
		private get;
		set;
	}

	public bool isEnabled
	{
		private get;
		set;
	}

	public GenericErrorDialogueButton()
		: this()
	{
	}

	private void Awake()
	{
		isEnabled = true;
	}

	private void OnClick()
	{
		if (isEnabled && onClicked != null)
		{
			onClicked();
		}
	}
}
