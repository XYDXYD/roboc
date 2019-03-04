using UnityEngine;

internal sealed class FlashWidgetOnPPrefs : MonoBehaviour
{
	public Color flashMinColor;

	public Color flashMaxColor;

	public float flashSpeed;

	public string keyName;

	private bool _flashSprite;

	private UIWidget _widget;

	private float _angle;

	public FlashWidgetOnPPrefs()
		: this()
	{
	}

	private void Awake()
	{
		_widget = this.GetComponent<UIWidget>();
	}

	private void OnEnable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_widget.set_color(flashMinColor);
		if (PlayerPrefs.HasKey(keyName))
		{
			_flashSprite = false;
		}
		else
		{
			_flashSprite = true;
		}
	}

	private void Update()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (_flashSprite)
		{
			_angle += Time.get_deltaTime() * flashSpeed;
			_widget.set_color(Color.Lerp(flashMinColor, flashMaxColor, Mathf.Sin(_angle)));
		}
	}
}
