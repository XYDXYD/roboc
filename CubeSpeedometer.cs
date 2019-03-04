using UnityEngine;

internal sealed class CubeSpeedometer : MonoBehaviour, IDisableOffScreen
{
	public UILabel speedText;

	public MeshRenderer speedBars;

	public float maxSpeed;

	public string stringFormat;

	public float unitMultiplier = 1f;

	private SpeedometerManagerBase _speedometerManager;

	public float visualUpdateRate = 0.5f;

	public float visualOffset = 0.2f;

	public Renderer _visualRenderer;

	private float _visualUpdateTimer;

	private Vector2 _currentVisualOffset = Vector2.get_zero();

	private Material _visualMaterial;

	public CubeSpeedometer()
		: this()
	{
	}//IL_0022: Unknown result type (might be due to invalid IL or missing references)
	//IL_0027: Unknown result type (might be due to invalid IL or missing references)


	internal void SetSpeedometerManager(SpeedometerManagerBase speedometerManager)
	{
		_speedometerManager = speedometerManager;
		_speedometerManager.Register(Populate, UpdateEnabled);
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
		_speedometerManager.Unregister(Populate, UpdateEnabled);
	}

	private void Populate(float speed)
	{
		speed *= unitMultiplier;
		float num = Mathf.Clamp01(speed / maxSpeed);
		speedText.set_text(speed.ToString(stringFormat));
		speedBars.get_material().SetFloat("_Cutoff", 1f - num);
	}

	private void UpdateEnabled(bool enabled)
	{
	}
}
