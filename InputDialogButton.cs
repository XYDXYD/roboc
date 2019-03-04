using System;
using UnityEngine;

internal sealed class InputDialogButton : MonoBehaviour
{
	public Action OnClicked
	{
		private get;
		set;
	}

	public bool IsEnabled
	{
		private get;
		set;
	}

	public InputDialogButton()
		: this()
	{
	}

	private void Awake()
	{
		OnClicked = delegate
		{
		};
		IsEnabled = true;
	}

	private void OnClick()
	{
		if (IsEnabled)
		{
			OnClicked();
		}
	}
}
