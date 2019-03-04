using UnityEngine;

internal class TestVTF : MonoBehaviour
{
	public Shader shader;

	private Camera _testCam;

	public TestVTF()
		: this()
	{
	}

	private void Start()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (SystemInfo.get_graphicsShaderLevel() < 30)
		{
			Object.DestroyImmediate(this.get_gameObject());
			return;
		}
		this.get_transform().set_position(new Vector3(0f, 100f, 0f));
		_testCam = this.get_gameObject().AddComponent<Camera>();
		_testCam.set_enabled(false);
		_testCam.set_targetTexture(new RenderTexture(1, 1, 0));
		_testCam.set_aspect(1f);
		GameObject val = GameObject.CreatePrimitive(4);
		Material material = new Material(shader);
		val.GetComponent<Renderer>().set_material(material);
		Transform transform = val.get_transform();
		transform.set_parent(this.get_transform());
		transform.set_localPosition(Vector3.get_forward());
		transform.set_localEulerAngles(new Vector3(270f, 0f, 0f));
	}

	private void Update()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		_testCam.Render();
		RenderTexture.set_active(_testCam.get_targetTexture());
		Texture2D val = new Texture2D(1, 1, 3, false);
		val.ReadPixels(new Rect(0f, 0f, 1f, 1f), 0, 0);
		RenderTexture.set_active(null);
		Color pixel = val.GetPixel(0, 0);
		if (pixel.g == 1f)
		{
			Shader.DisableKeyword("VTF_OFF");
			Shader.EnableKeyword("VTF_ON");
		}
		else
		{
			Shader.DisableKeyword("VTF_ON");
			Shader.EnableKeyword("VTF_OFF");
		}
		Object.DestroyImmediate(this.get_gameObject());
	}
}
