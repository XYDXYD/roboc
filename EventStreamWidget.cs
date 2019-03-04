using UnityEngine;

internal sealed class EventStreamWidget : MonoBehaviour
{
	public UILabel label;

	public UISprite background;

	public int pixelsBeforeText = 10;

	private Color originalBackgroundColour;

	public EventStreamWidget()
		: this()
	{
	}

	private void Awake()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		label.set_text("  ");
		originalBackgroundColour = background.get_color();
	}

	public void SetLabelText(string text)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (background != null && label != null)
		{
			label.set_text(text);
			Vector4 drawingDimensions = background.get_drawingDimensions();
			float w = drawingDimensions.w;
			Vector4 drawingDimensions2 = background.get_drawingDimensions();
			float num = w - drawingDimensions2.y;
			UISprite obj = background;
			float num2 = 2 * pixelsBeforeText;
			Vector2 printedSize = label.get_printedSize();
			obj.SetDimensions((int)(num2 + printedSize.x), (int)num);
		}
	}

	public void SetBackgroundColor(Color color)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		background.set_color(color);
	}

	public void ResetBackgroundColor()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		background.set_color(originalBackgroundColour);
	}

	public void SetAlpha(float alpha)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		UISprite obj = background;
		Color color = background.get_color();
		float r = color.r;
		Color color2 = background.get_color();
		float g = color2.g;
		Color color3 = background.get_color();
		obj.set_color(new Color(r, g, color3.b, alpha));
	}
}
