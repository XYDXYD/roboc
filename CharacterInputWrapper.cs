using Svelto.ES.Legacy;
using System;
using UnityEngine;

internal sealed class CharacterInputWrapper : MonoBehaviour, IHandleCharacterInput, IInputComponent, IComponent
{
	private float[] _data = new float[Enum.GetValues(typeof(CharacterInputAxis)).Length];

	public float horizontalAxis => _data[0];

	public float verticalAxis => _data[1];

	public bool jump => _data[2] != 0f;

	public bool crouch => _data[3] != 0f;

	public float mouseX => _data[4];

	public float mouseY => _data[5];

	public CharacterInputWrapper()
		: this()
	{
	}

	private void Awake()
	{
		Array.Clear(_data, 0, _data.Length);
	}

	private void LateUpdate()
	{
		Array.Clear(_data, 0, _data.Length);
	}

	public void HandleCharacterInput(InputCharacterData input)
	{
		_data = input.data;
	}
}
