using UnityEngine;

internal class UIWidgetMouseCursorManager : MonoBehaviour
{
	public Texture2D defaultMouseCursor;

	public Texture2D pressMouseCursor;

	public Texture2D hoverMouseCursor;

	public Vector2 defaultCursorHotSpot;

	public Vector2 pressCursorHotSpot;

	public Vector2 hoverCursorHotSpot;

	public bool returnToDefaultWhenUnfocused;

	public bool isFocused
	{
		get;
		private set;
	}

	public UIWidgetMouseCursorManager()
		: this()
	{
	}
}
