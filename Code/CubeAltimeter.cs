using UnityEngine;

internal sealed class CubeAltimeter : MonoBehaviour, IDisableOffScreen
{
	public UILabel heightText;

	public MeshRenderer heightBars;

	public float maxHeight = 2250f;

	public string stringFormat;

	public float unitMultiplier = 1f;

	private AltimeterManagerBase _altimeterManager;

	public float visualUpdateRate = 0.5f;

	public float visualOffset = 0.1f;

	public Renderer _visualRenderer;

	private float _visualUpdateTimer;

	private Vector2 _currentVisualOffset = Vector2.get_zero();

	private Material _visualMaterial;

	public CubeAltimeter()
		: this()
	{
	}//IL_002d: Unknown result type (might be due to invalid IL or missing references)
	//IL_0032: Unknown result type (might be due to invalid IL or missing references)


	internal void SetAltimeterManager(AltimeterManagerBase altimeterManager)
	{
		_altimeterManager = altimeterManager;
		_altimeterManager.Register(Populate, UpdateEnabled);
	}

	private void Start()
	{
		_visualMaterial = _visualRenderer.get_material();
	}

	private void Update()
	{
		UpdateRandomAnimation();
	}

	private void UpdateRandomAnimation()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		_visualUpdateTimer += Time.get_deltaTime();
		if (_visualUpdateTimer > visualUpdateRate)
		{
			ref Vector2 currentVisualOffset = ref _currentVisualOffset;
			currentVisualOffset.y += visualOffset;
			if (_currentVisualOffset.y >= 1f)
			{
				_currentVisualOffset.y = 0f;
			}
			_visualMaterial.SetTextureOffset("_MainTex", _currentVisualOffset);
			_visualUpdateTimer = 0f;
		}
	}

	private void OnDestroy()
	{
		if (_altimeterManager != null)
		{
			_altimeterManager.Unregister(Populate, UpdateEnabled);
		}
	}

	private void Populate(float height)
	{
		height *= unitMultiplier;
		float num = Mathf.Clamp01(height / maxHeight);
		heightText.set_text(height.ToString(stringFormat));
		heightBars.get_material().SetFloat("_Cutoff", 1f - num);
	}

	private void UpdateEnabled(bool enabled)
	{
	}
}
