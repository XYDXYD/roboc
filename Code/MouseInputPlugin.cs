using Svelto.ES.Legacy;
using System;
using UnityEngine;

internal sealed class MouseInputPlugin : IInputPlugin, IComponent
{
	private event Action<InputMouseData> OnMouseButton = delegate
	{
	};

	private event Action<float> OnMouseWheel = delegate
	{
	};

	public void RegisterComponent(IInputComponent component)
	{
		if (component is IHandleMouseButtons)
		{
			IHandleMouseButtons obj = component as IHandleMouseButtons;
			OnMouseButton += obj.HandleButton;
		}
		if (component is IHandleMouseWheel)
		{
			IHandleMouseWheel obj2 = component as IHandleMouseWheel;
			OnMouseWheel += obj2.HandleWheel;
		}
	}

	public void UnregisterComponent(IInputComponent component)
	{
		if (component is IHandleMouseButtons)
		{
			IHandleMouseButtons obj = component as IHandleMouseButtons;
			OnMouseButton -= obj.HandleButton;
		}
		if (component is IHandleMouseWheel)
		{
			IHandleMouseWheel obj2 = component as IHandleMouseWheel;
			OnMouseWheel -= obj2.HandleWheel;
		}
	}

	public void Execute()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetMouseButtonDown(0))
		{
			this.OnMouseButton(new InputMouseData(MouseButton.LEFT, MouseButtonState.CLICK, Vector2.op_Implicit(Input.get_mousePosition())));
		}
		if (Input.GetMouseButton(0))
		{
			this.OnMouseButton(new InputMouseData(MouseButton.LEFT, MouseButtonState.DOWN, Vector2.op_Implicit(Input.get_mousePosition())));
		}
		if (Input.GetMouseButtonDown(1))
		{
			this.OnMouseButton(new InputMouseData(MouseButton.RIGHT, MouseButtonState.CLICK, Vector2.op_Implicit(Input.get_mousePosition())));
		}
		if (Input.GetMouseButton(1))
		{
			this.OnMouseButton(new InputMouseData(MouseButton.RIGHT, MouseButtonState.DOWN, Vector2.op_Implicit(Input.get_mousePosition())));
		}
		if (Input.GetMouseButtonUp(1))
		{
			this.OnMouseButton(new InputMouseData(MouseButton.RIGHT, MouseButtonState.UP, Vector2.op_Implicit(Input.get_mousePosition())));
		}
		if (Input.GetMouseButtonDown(2))
		{
			this.OnMouseButton(new InputMouseData(MouseButton.MIDDLE, MouseButtonState.CLICK, Vector2.op_Implicit(Input.get_mousePosition())));
		}
		if (Input.GetMouseButton(2))
		{
			this.OnMouseButton(new InputMouseData(MouseButton.MIDDLE, MouseButtonState.DOWN, Vector2.op_Implicit(Input.get_mousePosition())));
		}
		if (Input.GetAxis("Mouse ScrollWheel") != 0f)
		{
			this.OnMouseWheel(Input.GetAxis("Mouse ScrollWheel"));
		}
	}
}
