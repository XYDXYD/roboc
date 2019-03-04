using UnityEngine;

internal sealed class ChatFlasher : MonoBehaviour
{
	public UIButton sprite;

	public UILabel label;

	private Color color1 = Color.get_black();

	public Color flashColor = Color.get_white();

	public float rate = 1f;

	private bool _shouldFlash;

	private float _switchTimer;

	private bool isFlashed;

	public ChatFlasher()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	//IL_000c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0011: Unknown result type (might be due to invalid IL or missing references)


	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		color1 = sprite.get_defaultColor();
	}

	public void SetNormalColor(Color c)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		color1 = c;
		sprite.set_defaultColor((!isFlashed) ? color1 : flashColor);
		sprite.set_isEnabled(false);
		sprite.set_isEnabled(true);
	}

	public void SetLabel(string name)
	{
		if (label != null)
		{
			label.set_text(name);
		}
	}

	private void Update()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (_shouldFlash)
		{
			_switchTimer += Time.get_deltaTime();
			if (_switchTimer > rate)
			{
				isFlashed = !isFlashed;
				sprite.set_defaultColor((!isFlashed) ? color1 : flashColor);
				sprite.set_isEnabled(false);
				sprite.set_isEnabled(true);
				_switchTimer -= rate;
			}
		}
	}

	public void ShouldFlash(bool b)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (b != _shouldFlash)
		{
			_shouldFlash = b;
			_switchTimer = 0f;
			isFlashed = b;
			sprite.set_defaultColor((!isFlashed) ? color1 : flashColor);
			sprite.set_isEnabled(false);
			sprite.set_isEnabled(true);
		}
	}
}
