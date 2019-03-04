using UnityEngine;

internal class ChangeCursorOnMouseOver : MonoBehaviour
{
	[SerializeField]
	private Texture2D normalCursor;

	[SerializeField]
	private Texture2D mouseOverCursor;

	[SerializeField]
	private Vector2 normalCursorHotSpot;

	[SerializeField]
	private Vector2 mouseOverCursorHotSpot;

	private bool _isOver;

	public ChangeCursorOnMouseOver()
		: this()
	{
	}

	private void Update()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (_isOver)
		{
			Cursor.SetCursor(mouseOverCursor, mouseOverCursorHotSpot, 0);
		}
		else
		{
			Cursor.SetCursor(normalCursor, normalCursorHotSpot, 0);
		}
	}

	private void OnHover(bool isOver)
	{
		_isOver = isOver;
	}
}
