using UnityEngine;

public class AnimatedProjector : MonoBehaviour
{
	private Material _material;

	public Color color;

	private Color _color;

	public AnimatedProjector()
		: this()
	{
	}

	private void Awake()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		_material = this.GetComponent<Projector>().get_material();
		_color = _material.GetColor("_Color");
		color = _color;
	}

	private void Update()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (color != _color)
		{
			_color = color;
			_material.SetColor("_Color", _color);
		}
	}
}
