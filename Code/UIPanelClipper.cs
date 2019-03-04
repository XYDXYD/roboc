using UnityEngine;

internal sealed class UIPanelClipper : MonoBehaviour
{
	public int leftBorder;

	public int rightBorder;

	public int topBorder;

	public int bottomBorder;

	public bool fixedWidth;

	public bool fixedHeight;

	private Vector2 _lastResolution;

	private UIPanel _panel;

	private BoxCollider _boxCollider;

	private Vector3 _startPos;

	private Vector3 _lastPos;

	private const float UI_COLLIDER_THICKNESSS = 0.1f;

	public UIPanelClipper()
		: this()
	{
	}

	private void Start()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		_panel = this.GetComponent<UIPanel>();
		_boxCollider = this.GetComponent<BoxCollider>();
		_startPos = this.get_transform().get_localPosition();
		_lastPos = this.get_transform().get_localPosition();
		UpdatePanelClips();
	}

	private void UpdatePanelClips()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = _startPos - this.get_transform().get_localPosition();
		Vector4 val2 = default(Vector4);
		val2._002Ector(-1f, -1f, -1f, -1f);
		val2.z = Screen.get_width() - leftBorder - rightBorder;
		val2.x = val2.z / 2f + val.x;
		val2.w = Screen.get_height() - topBorder - bottomBorder;
		val2.y = -1f * (val2.w / 2f) + val.y;
		Vector4 finalClipRegion = _panel.get_finalClipRegion();
		if (!fixedWidth)
		{
			finalClipRegion.x = val2.x;
			finalClipRegion.z = val2.z;
		}
		if (!fixedHeight)
		{
			finalClipRegion.y = val2.y;
			finalClipRegion.w = val2.w;
		}
		if (!fixedHeight || !fixedWidth)
		{
			_panel.set_baseClipRegion(finalClipRegion);
		}
		else
		{
			_panel.set_clipOffset(Vector2.op_Implicit(val));
		}
		if (_boxCollider != null)
		{
			UpdateBoxCollider(finalClipRegion);
		}
		_lastResolution.x = Screen.get_width();
		_lastResolution.y = Screen.get_height();
		_lastPos = this.get_transform().get_localPosition();
	}

	private void UpdateBoxCollider(Vector4 size)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Vector3 center = default(Vector3);
		center._002Ector(size.x, size.y, 10f);
		_boxCollider.set_center(center);
		Vector3 size2 = default(Vector3);
		size2._002Ector(size.z, size.w, 0.1f);
		_boxCollider.set_size(size2);
	}

	private void Update()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (_lastResolution.x != (float)Screen.get_width() || _lastResolution.y != (float)Screen.get_height())
		{
			UpdatePanelClips();
		}
		if (_lastPos != this.get_transform().get_localPosition())
		{
			UpdatePanelClips();
		}
	}
}
