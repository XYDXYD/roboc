using Mothership;
using Svelto.Command;
using Svelto.IoC;
using System;
using UnityEngine;

internal sealed class EnterPlanetSinglePlayerButton : MonoBehaviour
{
	[Inject]
	internal ICommandFactory commandFactory
	{
		private get;
		set;
	}

	public event Action<string> OnButtonPressed = delegate
	{
	};

	public EnterPlanetSinglePlayerButton()
		: this()
	{
	}

	private void OnClick()
	{
		this.OnButtonPressed("Button_SinglePlayer");
		commandFactory.Build<KickStartSinglePlayerCommand>().Execute();
	}
}
