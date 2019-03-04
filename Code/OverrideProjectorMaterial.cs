using UnityEngine;

internal class OverrideProjectorMaterial : MonoBehaviour
{
	[SerializeField]
	private Color _color;

	[SerializeField]
	private Color _outerColor;

	[SerializeField]
	private float _intensity;

	private static readonly int Color = Shader.PropertyToID("_Color");

	private static readonly int OuterColor = Shader.PropertyToID("_OuterColor");

	private static readonly int Intensity = Shader.PropertyToID("_Intensity");

	private Material _material;

	public OverrideProjectorMaterial()
		: this()
	{
	}

	private void Awake()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		Projector component = this.GetComponent<Projector>();
		Material val = new Material(component.get_material());
		Material obj = val;
		obj.set_name(obj.get_name() + " (Copy)");
		component.set_material(val);
		_material = val;
	}

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		_material.SetColor(Color, _color);
		_material.SetColor(OuterColor, _outerColor);
		_material.SetFloat(Intensity, _intensity);
	}

	private void OnDestroy()
	{
		Object.Destroy(_material);
	}
}
