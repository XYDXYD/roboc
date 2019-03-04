using Svelto.IoC;
using System;
using UnityEngine;

internal sealed class GenericInfoDialogueButton : MonoBehaviour
{
	[Inject]
	internal IGUIInputController inputController
	{
		private get;
		set;
	}

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

	public GenericInfoDialogueButton()
		: this()
	{
	}

	private void Awake()
	{
		onClicked = delegate
		{
		};
		isEnabled = true;
	}

	private void OnClick()
	{
		if (isEnabled)
		{
			if (inputController != null)
			{
				inputController.ToggleCurrentScreen();
			}
			if (onClicked != null)
			{
				onClicked();
			}
		}
	}
}
