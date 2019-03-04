using UnityEngine;

internal sealed class UIPosSpriteToLeftOfLabel : MonoBehaviour
{
	public UILabel label;

	public float fontSize;

	public Vector3 offset;

	private UISprite _sprite;

	private int _lastLength;

	public UIPosSpriteToLeftOfLabel()
		: this()
	{
	}

	private void Start()
	{
		_sprite = this.GetComponent<UISprite>();
	}

	private void Update()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (label != null && _sprite != null)
		{
			int length = label.get_text().Length;
			if (_lastLength != length)
			{
				_sprite.get_transform().set_localPosition(label.get_transform().get_localPosition() - new Vector3(fontSize * (float)length, 0f, 0f) - offset);
			}
		}
	}
}
