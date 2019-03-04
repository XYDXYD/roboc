using Svelto.ES.Legacy;
using System;
using UnityEngine;

internal sealed class MachineInputWrapper : MonoBehaviour, IHandleCharacterInput, IMachineControl, IInputComponent, IComponent
{
	private float[] _data = new float[Enum.GetValues(typeof(CharacterInputAxis)).Length];

	public float horizontalAxis => _data[0];

	public float forwardAxis => _data[1];

	public bool moveUpwards => _data[2] != 0f;

	public bool moveDown => _data[3] != 0f;

	public float mouseX => _data[4];

	public float mouseY => _data[5];

	public float fire1 => _data[6];

	public float fire2 => _data[7];

	public bool pulseAR
	{
		get
		{
			return _data[9] != 0f;
		}
		set
		{
			_data[9] = 0f;
		}
	}

	public bool toggleLight => _data[10] != 0f;

	public bool strafeLeft => _data[28] != 0f;

	public bool strafeRight => _data[29] != 0f;

	public bool taunt => _data[11] != 0f;

	public MachineInputWrapper()
		: this()
	{
	}

	private void Awake()
	{
		Array.Clear(_data, 0, _data.Length);
	}

	private void LateUpdate()
	{
	}

	public void HandleCharacterInput(InputCharacterData input)
	{
		_data = input.data;
	}
}
