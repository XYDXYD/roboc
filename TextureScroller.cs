using UnityEngine;

internal sealed class TextureScroller : MonoBehaviour
{
	public float scrollAmount;

	private Renderer[] _renderers;

	private Transform T;

	public TextureScroller()
		: this()
	{
	}

	private void Start()
	{
		T = this.get_transform();
		_renderers = this.GetComponentsInChildren<Renderer>();
	}

	private void Update()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (_renderers == null)
		{
			return;
		}
		scrollAmount = Mathf.Clamp01(scrollAmount);
		T.set_localScale(new Vector3(1f, 1f, 1f - scrollAmount));
		T.set_localPosition(new Vector3(0f, 0f, scrollAmount));
		Renderer[] renderers = _renderers;
		foreach (Renderer val in renderers)
		{
			if (val.get_material() != null)
			{
				Material material = val.get_material();
				material.set_mainTextureOffset(new Vector2(scrollAmount, 0f));
				material.set_mainTextureScale(new Vector2(1f - scrollAmount, 1f));
			}
		}
	}
}
