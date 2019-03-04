using UnityEngine;

internal class SetMaterialFloatParameter : MonoBehaviour
{
	public Material material;

	public string parameter;

	public float value;

	public SetMaterialFloatParameter()
		: this()
	{
	}

	private void Update()
	{
		if (material != null && material.HasProperty(parameter))
		{
			material.SetFloat(parameter, value);
		}
	}
}
