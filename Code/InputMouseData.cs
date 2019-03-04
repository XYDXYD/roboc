using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct InputMouseData
{
	public MouseButton button
	{
		get;
		private set;
	}

	public MouseButtonState state
	{
		get;
		private set;
	}

	public Vector2 pos
	{
		get;
		private set;
	}

	public InputMouseData(MouseButton _button, MouseButtonState _state, Vector2 _mousePos)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		this = default(InputMouseData);
		button = _button;
		state = _state;
		pos = _mousePos;
	}
}
