using Svelto.ES.Legacy;
using System.Collections.Generic;
using UnityEngine;

internal interface IHandleKeyboard : IInputComponent, IComponent
{
	void HandleKeyboard(Dictionary<KeyCode, bool> data);
}
