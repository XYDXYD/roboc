using UnityEngine;

internal sealed class UVScrollMaterialOverTime : MonoBehaviour
{
	public int materialIndex;

	public Vector2 scrollSpeed = Vector2.get_zero();

	public string textureName = "_MainTex";

	private Vector2 _uvOffset = Vector2.get_zero();

	public UVScrollMaterialOverTime()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	//IL_0017: Unknown result type (might be due to invalid IL or missing references)
	//IL_001c: Unknown result type (might be due to invalid IL or missing references)


	private void Update()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		_uvOffset += scrollSpeed * Time.get_deltaTime();
		if (this.GetComponent<Renderer>().get_enabled() && this.GetComponent<Renderer>().get_materials()[materialIndex] != null)
		{
			this.GetComponent<Renderer>().get_materials()[materialIndex].SetTextureOffset(textureName, _uvOffset);
		}
	}
}
